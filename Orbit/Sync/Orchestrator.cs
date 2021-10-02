using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.CheckIns;
using Serilog;
    
namespace Sync
{
    public record BatchInfo(string Url, Meta Meta, Links Links);

    public interface ISync
    {
        string To => "Activity";   
    }
    public interface ISync<TSource> : ISync
    {
        Task<ApiCursor<TSource>?> InitializeAsync(SyncContext context);
        Task<SyncStatus> ProcessItemAsync(TSource item);
    }

    public enum SyncStatus
    {
        Failed,
        Ignored,
        Exists,
        Success
    }

    public interface IMultiSync<TTopLevel, TSource> : ISync<TSource>
    {
        Task<ApiCursor<TTopLevel>> InitializeTopLevelAsync(SyncContext context);
        bool StopOnExists => false;
    }

    public class Orchestrator
    {
        private readonly ILogger _log;
        private readonly IServiceProvider _services;
        private readonly LogDbContext _logDb;
        private readonly SyncImplConfig _config;
        private readonly OrbitSync _orbitSync;
        private readonly List<SyncStats> _syncs;
        private FilesConfig _filesConfig;


        public Orchestrator(ILogger log, IServiceProvider services, LogDbContext logDb, SyncImplConfig config, 
            SyncDeps deps, FilesConfig filesConfig)
        {
            _log = log;
            _services = services;
            _logDb = logDb;
            _config = config;
            _filesConfig = filesConfig;
            _orbitSync = deps.OrbitSync;
            _syncs = new List<SyncStats>();
        }

        public async Task<int> All()
        {
            var returnCode = 0;
            _log.Information("Starting sync...");
            try
            {
                await PeopleToMembers();
                await CheckInsToActivities();
                await DonationsToActivities();
                await GroupAttendanceToActivities();
                await GroupMembershipToActivities();
                await NotesToActivities();
            }
            catch (Exception e)
            {
                _log.Fatal(e, "Unexpected error while performing sync");
                returnCode = 1;
            }

            await _logDb.SaveChangesAsync();

            await using var writer = new StreamWriter(File.Create(Path.Combine(_filesConfig.Root, "report.csv")));
            await using var csv = new CsvWriter(writer,new CsvConfiguration(CultureInfo.InvariantCulture));
            await csv.WriteRecordsAsync(_syncs);

            await using var text = new StreamWriter(File.Create(Path.Combine(_filesConfig.Root, "summary.txt")));

            var maxLength = _syncs.Select(s => s.Source.Length).Max();

            var format = $"{{0,-{maxLength+2}}}";
            var builder = new StringBuilder();
            foreach (var sync in _syncs)
            {
                builder
                    .AppendFormat(format, sync.Source + ":")
                    .Append("Pushed: ")
                    .Append(sync.Success);
                
                if (sync.Failed > 0)
                    builder.Append("; Failed: ")
                        .Append(sync.Failed);
                
                builder.Append(" in ")
                    .AppendFormat("{0:0.00} seconds", sync.TotalSeconds);
                builder.AppendLine();
                
            }

            _log.Information("Summary: \n{Summary}", builder);
            await text.WriteLineAsync(builder);
            return returnCode;
        }
        
        public async Task PeopleToMembers()
        {
            var impl = _services.GetRequiredService<PeopleToMembersSync>();
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
        
        public async Task CheckInsToActivities()
        {
            var impl = _services.GetRequiredService<CheckInsToActivitiesSync>();
            await Sync(impl);
        }
        
        public async Task GroupAttendanceToActivities()
        {
            var impl = _services.GetRequiredService<GroupAttendanceSync>();
            await MultiSync(impl);
        }
        
        public async Task GroupMembershipToActivities()
        {
            var impl = _services.GetRequiredService<GroupMembershipSync>();
            await MultiSync(impl);
        }

        private async Task MultiSync<TTopLevel, TSource>(IMultiSync<TTopLevel, TSource> impl) where TSource : EntityBase where TTopLevel : EntityBase
        {
            var record = new SyncStats(typeof(TSource).Name);
            _syncs.Add(record);
            
            _log.Information("Starting multi-sync from {SyncFrom} to {SyncChild} to {SyncTo}", 
                typeof(TTopLevel).Name, typeof(TSource).Name, impl.To);

            var context = await InitializeSync(impl);
            if (context == null) return;

            var children = context.OverallProgress.Children.ToDictionary(c => c.Type);

            var topCursor = await impl.InitializeTopLevelAsync(context);

            await RecordStart(context, topCursor);

            using (context.OverallProgress)
            {
                context.OverallProgress.Timer.Start();
                bool done = false;
                for (;!done;)
                {
                    foreach (var topLevel in topCursor!.Data!)
                    {
                        var type = context.OverallProgress.Type + ":" + topLevel.Id;

                        if (!children.TryGetValue(type, out var childProgress))
                        {
                            childProgress = new Progress()
                            {
                                Type = type
                            };
                            context.OverallProgress.Children.Add(childProgress);
                        }
                        else
                        {
                            if (childProgress.Complete) continue;
                        }

                        var childContext = context.MakeChild(childProgress);
                        childContext.SetData(topLevel);
                        
                        var cursor = await impl.InitializeAsync(childContext);
                        if (cursor == null) continue;
                            
                        await cursor.InitializeAsync();
                        var thisCount = cursor.Meta.TotalCount();
                        _log.Information("{CursorName} has {ThisCount} records", cursor.Name, thisCount);

                        bool didProcessAll = true;
                        if (cursor.Data!.Any())
                        {
                            didProcessAll = await ProcessCursorAsync(impl, cursor, childContext, context.OverallProgress);
                        }
                        
                        childContext.OverallProgress.Complete = true;
                        await _logDb.SaveChangesAsync();
                        if (!didProcessAll && impl.StopOnExists)
                        {
                            done = true;
                            break;
                        }
                    }

                    context.OverallProgress.NextUrl = topCursor.NextUrl;
                    await _logDb.SaveChangesAsync();
                    if (done) break;
                    if (!await topCursor.FetchNextAsync()) break;
                }    
            }
            _log.Information("Sync complete in {TotalTime}", context.OverallProgress.Timer.Elapsed);
            record.Record(context.OverallProgress);
            context.OverallProgress.Complete = true;
            await _logDb.SaveChangesAsync();
        }

        private async Task<SyncContext?> InitializeSync(ISync impl)
        {
            var name = impl.GetType().Name;
            var progress = await _logDb.Progress
                .Include(p => p.Children)
                .SingleOrDefaultAsync(p => p.Type == name);

            if (progress != null)
            {
                if (progress.Complete)
                {
                    _log.Information("Sync previously completed successfully, skipping");
                    return null;
                }
                _log.Information("Resuming sync at url {Url}", progress.NextUrl);
            }
            else
            {
                _log.Information("Starting new sync");
                progress = new Progress()
                {
                    Type = name
                };
            }

            var context = new SyncContext()
            {
                OverallProgress = progress,
                NextUrl = progress.NextUrl
            };
            return context;
        }

        private async Task Sync<TSource>(ISync<TSource> impl) where TSource : EntityBase
        {
            var record = new SyncStats(typeof(TSource).Name);
            _syncs.Add(record);
            _log.Information("Starting sync from {SyncFrom} to {SyncTo}", typeof(TSource).Name, impl.To);
            var context = await InitializeSync(impl);
            if (context == null) return;
            
            var cursor = await impl.InitializeAsync(context);
            
            await RecordStart(context, cursor!);

            using (context.OverallProgress)
            {
                context.OverallProgress.Timer.Start();
                await ProcessCursorAsync(impl, cursor!, context, context.OverallProgress);
            }
            _log.Information("Sync complete in {TotalTime}", context.OverallProgress.Timer.Elapsed);
            record.Record(context.OverallProgress);
            context.OverallProgress.Complete = true;
            await _logDb.SaveChangesAsync();
        }

        private async Task RecordStart<TSource>(SyncContext context, ApiCursor<TSource> cursor) where TSource : EntityBase
        {
            await cursor.InitializeAsync();
            if (context.OverallProgress.NextUrl == null)
            {
                context.OverallProgress.NextUrl = cursor.NextUrl;
                _logDb.Progress.Add(context.OverallProgress);
                await _logDb.SaveChangesAsync();
            }

            _log.Information("Found {QueueCount} {EntityType} to sync: {Url}",
                cursor.Meta.TotalCount(), typeof(TSource).Name, cursor.NextUrl);
            _log.Information("Using batch size of {BatchSize} for {PageCount} pages",
                cursor.Meta.Count(), cursor.Meta.PageCount());
        }

        private enum CursorState
        {
            ProcessAll,
            FindBatch,
            EnsureCompleteness
        }
        private async Task<bool> ProcessCursorAsync<TSource>(ISync<TSource> impl, ApiCursor<TSource> cursor, 
            SyncContext context, Progress progress)
            where TSource : EntityBase
        {
            var didProcessAll = true;
            var state = _config.Mode == SyncMode.Seek ? CursorState.FindBatch : CursorState.ProcessAll;
            for (;;)
            {
                using var batchStats = new Progress();
                context.BatchProgress = batchStats;
                batchStats.Timer.Start();

                var data = cursor.Data!.ToList();
                for (var i = 0; i < data.Count; i++)
                {
                    var item = data![i];
                    if (state != CursorState.ProcessAll)
                    {
                        var existing = await _orbitSync.GetActivity(item);
                        if (existing != null)
                        {
                            if (state == CursorState.FindBatch)
                            {
                                batchStats.Skipped += data.Count - i;
                                break;    
                            }

                            if (state == CursorState.EnsureCompleteness)
                            {
                                batchStats.Skipped++;
                                continue;
                            }
                        }
                        else
                        {
                            if (state == CursorState.FindBatch)
                            {
                                await cursor.FetchPreviousAsync();
                                await cursor.FetchPreviousAsync();
                                i = 0;
                                state = CursorState.EnsureCompleteness;
                                continue;
                            }

                            state = CursorState.ProcessAll;
                        }
                    }
                    
                    var result = await impl.ProcessItemAsync(item);
                    batchStats.RecordItem(result);
                    if (result == SyncStatus.Exists && _config.KeyExistsMode == KeyExistsMode.Stop)
                    {
                        batchStats.Complete = true;
                        didProcessAll = false;
                        break;
                    }
                }

                progress.TotalTime += progress.Timer.ElapsedMilliseconds;
                Report<TSource>(batchStats, progress, cursor.Meta);
                await _logDb.SaveChangesAsync();

                if (batchStats.Complete) break;
                if (!await cursor.FetchNextAsync())
                {
                    if (state == CursorState.FindBatch)
                    {
                        state = CursorState.EnsureCompleteness;
                        // make sure the entire last batch was uploaded
                        batchStats.Skipped -= data.Count;
                    }
                    else break;
                }
                
                context.OverallProgress.NextUrl = cursor.NextUrl;
                await _logDb.SaveChangesAsync();
            }

            return didProcessAll;
        }

        private void Report<TSource>(Progress batchStats, Progress progress, Meta meta)
            where TSource : EntityBase
        {
            _log.Information(
                "Batch: processed {TotalRecords} of type {RecordType} in {TotalSeconds} at {RecordsPerSecond:F1} records/s; LastDate {LastDate}",
                batchStats.Total, typeof(TSource).Name, batchStats.SecondsElapsed, batchStats.RecordsPerSecond, _orbitSync.LastDate);

            progress.Accumulate(batchStats);
            _log.Information(
                "Overall: elapsed: {Elapsed} s; processed/queued: {TotalProcessed}/{QueueCount}; " +
                "success: {Success}; skipped: {Skipped}; failed: {Failed}; RecordsPerSecond: {RecordsPerSecond:F1}",
                progress.SecondsElapsed, progress.Total, meta.TotalCount(), progress.Success,
                progress.Skipped,
                progress.Failed, progress.RecordsPerSecond);
        }
    }

    public class SyncStats
    {
        public SyncStats(string source)
        {
            Source = source;
        }
        public string Source { get; set; }
        public long SourceTotal => Success + Failed + Skipped;
        public long Success { get; set; }
        public long Failed { get; set; }
        public long Skipped { get; set; }
        public double TotalSeconds { get; set; }

        public void Record(Progress o)
        {
            Success = o.Success;
            Skipped = o.Skipped;
            Failed = o.Failed;
            TotalSeconds = Math.Round(o.Timer.Elapsed.TotalSeconds, 1);
        }
    }

    public class SyncContext
    {
        private readonly Dictionary<object, object> _data;
        private SyncContext(Dictionary<object, object> data)
        {
            _data = new Dictionary<object, object>(data);
        }
        public SyncContext() : this(new Dictionary<object, object>())
        {
            
        }
        
        public string? NextUrl { get; set; }
        public Progress OverallProgress { get; set; }
        public Progress BatchProgress { get; set; }

        public void SetData<T>(T data) => _data[typeof(T)] = data!; 
        public T GetData<T>() => (T)_data[typeof(T)]!;

        public SyncContext MakeChild(Progress progress)
        {
            return new SyncContext(_data)
            {
                NextUrl = progress.NextUrl,
                OverallProgress = progress
            };
        }
    }
}