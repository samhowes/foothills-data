using System;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Sync
{
    public record SyncConfig(long MaxCount);
    public record BatchInfo(string Url, Meta Meta, Links Links);
    
    interface ISync
    {
        string From { get; }
        string To { get; }

        Task<BatchInfo> GetInitialDataAsync(string? nextUrl);
        Task ProcessBatchAsync(Progress progress);
        Task<string?> GetNextBatchAsync();
    }

    public class Synchronizer
    {
        private readonly ILogger _log;
        private readonly IServiceProvider _services;
        private readonly LogDbContext _logDb;
        private readonly SyncConfig _config;


        public Synchronizer(ILogger log, IServiceProvider services, LogDbContext logDb, SyncConfig config)
        {
            _log = log;
            _services = services;
            _logDb = logDb;
            _config = config;
        }

        public async Task PeopleToMembers()
        {
            var impl = _services.GetRequiredService<PeopleToMembersSync>();
            await Sync(impl);
        }

        private async Task Sync(ISync impl)
        {
            _log.Information("Starting sync from {SyncFrom} to {SyncTo}", impl.From, impl.To);
            var name = impl.GetType().Name;
            var progress = await _logDb.Progress.SingleOrDefaultAsync(p => p.Type == name);

            if (progress != null)
            {
                _log.Information("Resuming sync at url {Url}", progress.NextUrl);
            }
            else
            {
                _log.Information("Starting new sync");
            }
            
            var initialData = await impl.GetInitialDataAsync(progress?.NextUrl);

            if (progress == null)
            {
                progress = new Progress()
                {
                    Type = name,
                    NextUrl = initialData.Url
                };
                _logDb.Progress.Add(progress);
                await _logDb.SaveChangesAsync();
            }
            
            _log.Information("Found {QueueCount} {EntityType} to sync: {Url}",
                initialData.Meta.TotalCount(), impl.From, initialData.Links.Self());
            _log.Information("Using batch size of {BatchSize} for {PageCount} pages",
                initialData.Meta.Count(), initialData.Meta.PageCount());

            using (progress)
            {
                progress.Timer.Start();
                for (;;)
                {
                    using var batchStats = new Progress();

                    await impl.ProcessBatchAsync(batchStats);
                    _log.Information(
                        "Batch: processed {TotalRecords} {RecordType} in {TotalSeconds} at {RecordsPerSecond} records/s",
                        batchStats.Total, impl.From, batchStats.SecondsElapsed, batchStats.RecordsPerSecond);

                    progress.Accumulate(batchStats);
                    _log.Information(
                        "Overall: elapsed: {Elapsed} s; processed/queued: {TotalProcessed}/{QueueCount}; " +
                        "success: {Success}; skipped: {Skipped}; failed: {Failed}; RecordsPerSecond: {RecordsPerSecond}",
                        progress.SecondsElapsed, progress.Total, initialData.Meta.TotalCount(), progress.Success, progress.Skipped,
                        progress.Failed, progress.RecordsPerSecond);

                    progress.NextUrl = await impl.GetNextBatchAsync();
                    progress.TotalTime += progress.Timer.ElapsedMilliseconds;
                    
                    await _logDb.SaveChangesAsync();

                    if (_config.MaxCount > 0 && progress.Success > _config.MaxCount)
                    {
                        _log.Information("MaxCount of {MaxCount} exceeded with {SuccessCount}", 
                            _config.MaxCount, progress.Success);
                        break;
                    }
                    if (progress.NextUrl == null) break;
                }
            }

            _log.Information("Sync complete");
        }

        public async Task CheckInsToActivities()
        {
            var impl = _services.GetRequiredService<CheckInsToActivitiesSync>();
            await Sync(impl);
        }
    }
}