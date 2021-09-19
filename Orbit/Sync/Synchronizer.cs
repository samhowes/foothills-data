using System;
using System.Collections.Generic;
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
        
        public async Task CheckInsToActivities()
        {
            var impl = _services.GetRequiredService<CheckInsToActivitiesSync>();
            await Sync(impl);
        }

        public async Task DonationsToActivities()
        {
            var impl = _services.GetRequiredService<DonationsToActivitiesSync>();
            await Sync(impl);
        }
        
        public async Task NotesToActivities()
        {
            var impl = _services.GetRequiredService<NotesToActivitiesSync>();
            await Sync(impl);
        }
        
        public async Task GroupAttendanceToActivities()
        {
            var impl = _services.GetRequiredService<GroupAttendanceSync>();
            await Sync(impl);
        }

        private async Task Sync<TSource>(Sync<TSource> impl) where TSource : EntityBase
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
            
            var batch = await impl.GetInitialDataAsync(progress?.NextUrl);

            if (progress == null)
            {
                progress = new Progress()
                {
                    Type = name,
                    NextUrl = batch.Links.Self()
                };
                _logDb.Progress.Add(progress);
                await _logDb.SaveChangesAsync();
            }
            
            _log.Information("Found {QueueCount} {EntityType} to sync: {Url}",
                batch.Meta.TotalCount(), impl.From, batch.Links.Self());
            _log.Information("Using batch size of {BatchSize} for {PageCount} pages",
                batch.Meta.Count(), batch.Meta.PageCount());

            using (progress)
            {
                progress.Timer.Start();
                await ProcessAllBatchesAsync(impl, batch, progress);
            }
            _log.Information("Sync complete in {TotalTime}", progress.Timer.Elapsed);
        }

        private async Task ProcessAllBatchesAsync<TSource>(Sync<TSource> impl, DocumentRoot<List<TSource>> batch, Progress progress)
            where TSource : EntityBase
        {
            for (;;)
            {
                using var batchStats = new Progress();
                batchStats.Timer.Start();

                foreach (var item in batch.Data)
                {
                    await impl.ProcessBatchAsync(batchStats, item);
                }

                await impl.AfterEachBatchAsync();
                
                progress.TotalTime += progress.Timer.ElapsedMilliseconds;
                Report(impl, batchStats, progress, batch);
                await _logDb.SaveChangesAsync();
                if (_config.MaxCount > 0 && progress.Success >= _config.MaxCount)
                {
                    _log.Information("MaxCount of {MaxCount} exceeded with {SuccessCount}",
                        _config.MaxCount, progress.Success);
                    break;
                }

                var nextUrl = batch.Links.Next();
                if (string.IsNullOrEmpty(nextUrl)) break;

                batch = await impl.PlanningCenterClient.GetAsync<List<TSource>>(nextUrl);

                progress.NextUrl = nextUrl;
                await _logDb.SaveChangesAsync();
            }
        }

        private void Report<TSource>(Sync<TSource> impl, Progress batchStats, Progress progress, DocumentRoot<List<TSource>> batch)
            where TSource : EntityBase
        {
            _log.Information(
                "Batch: processed {TotalRecords} of type {RecordType} in {TotalSeconds} at {RecordsPerSecond:F1} records/s; LastDate {LastDate}",
                batchStats.Total, impl.From, batchStats.SecondsElapsed, batchStats.RecordsPerSecond, impl.LastDate);

            progress.Accumulate(batchStats);
            _log.Information(
                "Overall: elapsed: {Elapsed} s; processed/queued: {TotalProcessed}/{QueueCount}; " +
                "success: {Success}; skipped: {Skipped}; failed: {Failed}; RecordsPerSecond: {RecordsPerSecond:F1}",
                progress.SecondsElapsed, progress.Total, batch.Meta.TotalCount(), progress.Success,
                progress.Skipped,
                progress.Failed, progress.RecordsPerSecond);
        }

        public async Task GroupToActivities()
        {
            var impl = _services.GetRequiredService<GroupMembershipSync>();
            await Sync(impl);
        }
    }
}