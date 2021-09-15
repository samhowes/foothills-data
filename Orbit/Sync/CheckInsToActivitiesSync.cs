using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;
using Serilog;

namespace Sync
{
    public record AddActivity(UploadActivity UploadActivity, Identity Identity);

    public enum SyncMode
    {
        Create,
        Update
    }

    public record CheckInsToActivitiesSyncConfig(
        SyncMode Mode = SyncMode.Create,
        params (string Name, string Value)[] Params
    );

    public class OrbitCache
    {
        public Dictionary<string, string> PersonToMember { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> CheckInToActivity { get; } = new Dictionary<string, string>();
    }

    public static class OrbitUtil
    {
        public static string? TrimLink(string? href)
        {
            if (href == null) return href;
            var index = href.IndexOf('/');
            index = href.IndexOf('/', index + 1);
            return href[(index + 1)..];
        }
    }
    
    public class CheckInsToActivitiesSync : ISync
    {
        private readonly CheckInsClient _checkInsClient;
        private DocumentRoot<List<CheckIn>> _checkIns = null!;
        private LogDbContext _logDb;
        private readonly OrbitApiClient _orbitClient;
        private ILogger _log;
        private readonly CheckInsToActivitiesSyncConfig _config;
        private Event _worship = null!;
        private HashSet<string> _ignoredEventIds = null!;
        private Blob _blob = null!;
        private OrbitCache _cache = new OrbitCache();

        public CheckInsToActivitiesSync(
            CheckInsClient checkInsClient, 
            LogDbContext logDb, 
            OrbitApiClient orbitClient, 
            ILogger log,
            CheckInsToActivitiesSyncConfig config)
        {
            _checkInsClient = checkInsClient;
            _logDb = logDb;
            _orbitClient = orbitClient;
            _log = log;
            _config = config;
        }

        public string From => "CheckIns";
        public string To => "Activities";
        public async Task<BatchInfo> GetInitialDataAsync(string? nextUrl)
        {
            var urlBase = "events";
            var worships = await _checkInsClient.GetAsync<List<Event>>(urlBase, 
                ("where[name]", "Sunday Attendance"),
                ("include", "event_periods"));
            
            if (worships.Data.Count != 1)
            {
                throw new PlanningCenterException(
                    $"Expected to find 1 event titled 'Sunday Attendance', but found {worships.Meta.TotalCount()} " +
                    $"instead: {string.Join(",", worships.Data.Select(w => w.Name))}");
            }
            
            _worship = worships.Data.Single();

            if (nextUrl != null)
            {
                _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>(nextUrl);
            }
            else
            {
                var pars = new Dictionary<string, string>()
                {
                    {"include", "event_times"},
                    {"order", "created_at"}
                };
                if (_config.Params.Any())
                {
                    foreach (var (name, value) in _config.Params)
                        pars[name] = value;
                }
                _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>("check_ins",
                    pars.Select(p => (p.Key, p.Value)).ToArray());
            }

            const string skippedKey = "skippedEvents";
            _blob = await _logDb.Blobs.SingleOrDefaultAsync(b => b.Key == skippedKey);
            if (_blob == null)
            {
                _blob = new Blob() {Key = skippedKey, Value = ""};
            }

            _ignoredEventIds = new HashSet<string>(_blob.Value.Split(","));
            
            return new BatchInfo(_checkIns.Links.Self(), _checkIns.Meta, _checkIns.Links);
        }

        public async Task ProcessBatchAsync(Progress progress)
        {
            foreach (var checkIn in _checkIns.Data)
            {
                foreach (var eventTime in checkIn.EventTimes.Data)
                {
                    var eventId = eventTime.Event.Id;
                    if (eventId != _worship.Id)
                    {
                        if (_ignoredEventIds.Add(eventId))
                        {
                            var details = await _checkInsClient.GetAsync<Event>($"events/{eventId}");
                            var self = details.Data.Links.Self();
                            details.Data.Links = null!;
                            _log.Information("Ignoring event {EventLink}: {EventJson}", self, JsonConvert.SerializeObject(details.Data));
                            _blob.Value = string.Join(",", _ignoredEventIds);
                            await _logDb.SaveChangesAsync();
                        }

                        progress.Skipped++;
                        continue;
                    }

                    if (checkIn.Person == null)
                    {
                        _log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                        _logDb.Mappings.Add(new Mapping()
                        {
                            PlanningCenterId = checkIn.Id,
                            Type = nameof(CheckIn),
                            Error = $"Missing person for {checkIn.Links.Self()}"
                        });
                        await _logDb.SaveChangesAsync();
                        continue;
                    }

                    var activity = new UploadActivity()
                        {
                            Title = $"Checked in for Worship for {eventTime.Name}",
                            Key = ActivityKey(checkIn),
                            Link =
                                $"https://check-ins.planningcenteronline.com/event_periods/{checkIn.EventPeriod.Id}/check_ins/{checkIn.Id}",
                            LinkText = "CheckIn",
                            ActivityType = "Online Worship Attendance",
                            OccurredAt = checkIn.CreatedAt,
                            Tags = new List<string>()
                            {
                                "channel:Worship"
                            },
                            Weight = "5"
                        };

                    var identity = new Identity(source: Constants.PlanningCenterSource)
                    {
                        Uid = checkIn.Person.Id,
                    };
                    
                    try
                    {
                        if (_config.Mode == SyncMode.Create)
                        {
                            await CreateActivity(activity, identity);
                        }
                        else
                        {
                            if (!_cache.PersonToMember.TryGetValue(checkIn.Person.Id, out var memberSlug))
                            {
                                var member = await _orbitClient.GetAsync<Member>("members/find",
                                    ("source", Constants.PlanningCenterSource),
                                    ("uid", checkIn.Person.Id));

                                memberSlug = member.Data.Slug;
                                _cache.PersonToMember[checkIn.Person.Id] = memberSlug;
                            }

                            if (!_cache.CheckInToActivity.TryGetValue(checkIn.Id, out var activityId))
                            {
                                string? nextUrl = null;
                                for (;;)
                                {
                                    var batch = await _orbitClient.GetAsync<List<ActivityBase>>(
                                        nextUrl ?? $"members/{memberSlug}/activities");
                                    foreach (var maybe in batch.Data)
                                    {
                                        if (string.Equals(maybe.Key, activity.Key, StringComparison.OrdinalIgnoreCase))
                                            activityId = maybe.Id;

                                        var checkInId = CheckInId(maybe.Key);
                                        if (checkInId == null) continue;
                                        
                                        _cache.CheckInToActivity[checkInId] = maybe.Id;
                                    }

                                    nextUrl = OrbitUtil.TrimLink(batch.Links.Next());
                                    if (activityId != null || string.IsNullOrEmpty(nextUrl)) break;
                                }
                            }

                            if (activityId == null)
                            {
                                await CreateActivity(activity, identity);
                            }
                            else
                            {
                                activity.Description = "";
                                var updated = await _orbitClient.PutAsync<DocumentRoot<UploadActivity>>(
                                    $"members/{memberSlug}/activities/{activityId}", activity);    
                            }
                            
                        }
                        
                        progress.Success++;
                    }
                    catch (Exception ex)
                    {
                        string errorMessage;
                        if (ex is ApiException orbitException)
                        {
                            if (ex.Message.Contains("has already been taken"))
                            {
                                _log.Debug("Skipping already uploaded activity");
                                progress.Skipped++;
                                continue;
                            }
                            _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", checkIn.Id, orbitException.Message);
                            errorMessage = orbitException.Message;
                        }
                        else
                        {
                            _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", checkIn.Id);
                            errorMessage = ex.StackTrace;
                        }

                        var mapping = new Mapping()
                        {
                            PlanningCenterId = checkIn.Id,
                            Type = nameof(CheckIn),
                            Error = errorMessage
                        };

                        _logDb.Add(mapping);
                        await _logDb.SaveChangesAsync();
                        
                        progress.Failed++;
                    }

                }
            }
            await _logDb.SaveChangesAsync();
        }

        private async Task CreateActivity(UploadActivity uploadActivity, Identity identity)
        {
            var created = await _orbitClient.PostAsync<DocumentRoot<UploadActivity>>("activities", 
                new AddActivity(uploadActivity, identity));
        }

        private const string ActivityKeyPrefix = "check_ins/";
        private static string ActivityKey(CheckIn checkIn)
        {
            return $"{ActivityKeyPrefix}{checkIn.Id}";
        }
        private static string? CheckInId(string activityKey)
        {
            return !activityKey.StartsWith(ActivityKeyPrefix) 
                ? null : 
                activityKey[ActivityKeyPrefix.Length..];
        }

        public async Task<string?> GetNextBatchAsync()
        {
            if (string.IsNullOrEmpty(_checkIns.Links.Next())) return null;
            _checkIns = await _checkInsClient.GetAsync<List<CheckIn>>(_checkIns.Links.Next()!);
            return _checkIns.Links.Self();
        }
    }

    public class PlanningCenterException : Exception
    {
        public PlanningCenterException(string message) : base(message)
        {
        }
    }
}