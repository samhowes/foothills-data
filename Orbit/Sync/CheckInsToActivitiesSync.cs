using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public record CheckInsToActivitiesSyncConfig 
    {
        public (string Name, string Value)[] Params { get; }

        public CheckInsToActivitiesSyncConfig(params (string Name, string Value)[] @params)
        {
            Params = @params;
        }
    }

    public class CheckInsToActivitiesSync : Sync<CheckIn>
    {
        private readonly CheckInsClient _checkInsClient;
        private DocumentRoot<List<CheckIn>> _checkIns = null!;
        private readonly CheckInsToActivitiesSyncConfig _config;
        private Event _worship = null!;
        private HashSet<string> _ignoredEventIds = null!;
        private Blob _blob = null!;

        public CheckInsToActivitiesSync(
            SyncDeps deps,
            CheckInsClient checkInsClient,
            CheckInsToActivitiesSyncConfig config)
            : base(deps, checkInsClient)
        {
            _checkInsClient = checkInsClient;
            _config = config;
        }

        public override async Task<DocumentRoot<List<CheckIn>>> GetInitialDataAsync(string? nextUrl)
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
            _blob = await Deps.LogDb.Blobs.SingleOrDefaultAsync(b => b.Key == skippedKey);
            if (_blob == null)
            {
                _blob = new Blob() {Key = skippedKey, Value = ""};
            }

            _ignoredEventIds = new HashSet<string>(_blob.Value.Split(","));

            return _checkIns;
        }

        public override async Task ProcessBatchAsync(Progress progress, CheckIn checkIn)
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
                        Log.Information("Ignoring event {EventLink}: {EventJson}", self,
                            JsonConvert.SerializeObject(details.Data));
                        _blob.Value = string.Join(",", _ignoredEventIds);
                        await Deps.LogDb.SaveChangesAsync();
                    }

                    progress.Skipped++;
                    continue;
                }

                if (checkIn.Person == null)
                {
                    Log.Warning("No person associated with CheckIn {CheckInId}", checkIn.Id);
                    Deps.LogDb.Mappings.Add(new Mapping()
                    {
                        PlanningCenterId = checkIn.Id,
                        Type = nameof(CheckIn),
                        Error = $"Missing person for {checkIn.Links.Self()}"
                    });
                    await Deps.LogDb.SaveChangesAsync();
                    continue;
                }

                var activity = new UploadActivity(
                    "Worship",
                    "Online Worship Attendance",
                    OrbitUtil.ActivityKey(checkIn),
                    checkIn.CreatedAt,
                    5m,
                    $"Checked in for Worship for {eventTime.Name}",
                    $"https://check-ins.planningcenteronline.com/event_periods/{checkIn.EventPeriod.Id}/check_ins/{checkIn.Id}",
                    "CheckIn"
                );

                await UploadActivity(progress, checkIn, activity, checkIn.Person.Id);
            }
        }
    }

    public class PlanningCenterException : Exception
    {
        public PlanningCenterException(string message) : base(message)
        {
        }
    }
}