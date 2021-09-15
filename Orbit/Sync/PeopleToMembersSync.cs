using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;
using Serilog;

namespace Sync
{
    public static class Constants
    {
        public const string PlanningCenterSource = "planningcenter";
    }
    
    public class PeopleToMembersSync : Sync<Person>
    {
        private readonly PeopleClient _peopleClient;
        private readonly LogDbContext _logDb;
        private readonly OrbitApiClient _orbitClient;
        private readonly ILogger _log;
        private Dictionary<string,Mapping> _existing = null!;
        private DocumentRoot<List<Person>> _people = null!;
        
        public PeopleToMembersSync(PeopleClient peopleClient, LogDbContext logDb, OrbitApiClient orbitClient,
            ILogger log, DataCache cache)
            : base(new SyncImplConfig(), orbitClient, cache, log, logDb, peopleClient)
        {
            _peopleClient = peopleClient;
            _logDb = logDb;
            _orbitClient = orbitClient;
            _log = log;
        }

        public override string From => "People";
        public override string To => "Members";

        public override async Task<DocumentRoot<List<Person>>> GetInitialDataAsync(string? nextUrl)
        {
            _people = await _peopleClient.GetAsync<List<Person>>(nextUrl ?? "people");
            _existing = await _logDb.Mappings
                .Where(m => m.Type == nameof(Person))
                .ToDictionaryAsync(m => m.PlanningCenterId!);
            return _people;
        }

        public override async Task ProcessBatchAsync(Progress progress, Person person)
        {
            if (_existing.TryGetValue(person.Id, out var mapping))
            {
                progress.Skipped++;
                return;
            }

            mapping = new Mapping()
            {
                PlanningCenterId = person.Id,
            };
            var tags = new List<string>();
            if (person.Child == "true")
                tags.Add("child");

            var member = new UpsertMember()
            {
                Birthday = person.Birthdate,
                Name = person.Name,
                Slug = person.Id,
                TagsToAdd = string.Join(",", tags),
                Identity = new Identity(source: Constants.PlanningCenterSource)
                {
                    Email = $"{person.Id}@foothillsuu.org",
                    Name = person.Name,
                    Uid = person.Id,
                    Url = person.Links.Self()!,
                }
            };

            try
            {
                var created = await _orbitClient.PostAsync<Member>("members", member);
                mapping.OrbitId = created.Data.Id;
                progress.Success++;
            }
            catch (ApiException orbitEx)
            {
                mapping.Error = orbitEx.Message;
                _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                    mapping.Error);
                progress.Failed++;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                mapping.Error = ex.ToString();
                progress.Failed++;
            }

            _logDb.Mappings.Add(mapping);
        }
    }
}