using System;
using System.Threading.Tasks;
using JsonApi;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Sync
{
    interface ISync
    {
        string From { get; }
        string To { get; }

        Task<Response> GetInitialDataAsync();
        Task ProcessBatchAsync(Stats stats);
        Task<bool> GetNextBatchAsync();
    }
    
    public class Synchronizer
    {
        private readonly ILogger _log;
        private readonly IServiceProvider _services;


        public Synchronizer(ILogger log, IServiceProvider services)
        {
            _log = log;
            _services = services;
        }
        
        public async Task PeopleToMembers()
        {
            var impl = _services.GetRequiredService<PeopleToMembersSync>();
            await Sync(impl);
        }
        
        private async Task Sync(ISync impl)
        {
            _log.Information("Starting sync from {SyncFrom} to {SyncTo}", impl.From, impl.To);
            using var stats = new Stats();
            var initialData = await impl.GetInitialDataAsync();

            _log.Information("Found {QueueCount} {EntityType} to sync: {Url}",
                initialData.Meta.TotalCount, impl.From, initialData.Links.Self);
            _log.Information("Using batch size of {BatchSize} for {PageCount} pages", 
                initialData.Meta.Count, initialData.Meta.PageCount);

            for (;;)
            {
                using var batchStats = new Stats();
                var hasMore = await impl.GetNextBatchAsync();
                if (!hasMore) break;
                
                await impl.ProcessBatchAsync(batchStats);
                _log.Information(
                    "Batch: processed {TotalRecords} {RecordType} in {TotalSeconds} at {RecordsPerSecond} records/s",
                    batchStats.Total, impl.From, batchStats.SecondsElapsed, batchStats.RecordsPerSecond);

                stats.Accumulate(batchStats);
                _log.Information(
                    "Overall: elapsed: {Elapsed} s; processed/queued: {TotalProcessed}/{QueueCount}; " +
                    "success: {Success}; skipped: {Skipped}; failed: {Failed}; RecordsPerSecond: {RecordsPerSecond}",
                    stats.SecondsElapsed, stats.Total, initialData.Meta.TotalCount, stats.Success, stats.Skipped,
                    stats.Failed, stats.RecordsPerSecond);
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