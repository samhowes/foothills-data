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
    
    public class PeopleToMembersSync : ISync
    {
        private readonly PeopleClient _peopleClient;
        private readonly LogDbContext _logDb;
        private readonly OrbitApiClient _orbitClient;
        private readonly ILogger _log;
        private Dictionary<string,Mapping> _existing;
        private DocumentRoot<List<Person>> _people;
        
        public PeopleToMembersSync(PeopleClient peopleClient, LogDbContext logDb, OrbitApiClient orbitClient, ILogger log)
        {
            _peopleClient = peopleClient;
            _logDb = logDb;
            _orbitClient = orbitClient;
            _log = log;
        }

        public string From => "People";
        public string To => "Members";

        public async Task<(Meta Meta, Links Links)> GetInitialDataAsync()
        {
            _people = await _peopleClient.GetAsync<List<Person>>("people");
            _existing = await _logDb.Mappings
                .Where(m => m.Type == nameof(Person))
                .ToDictionaryAsync(m => m.PlanningCenterId);
            return (_people.Meta, _people.Links);
        }

        public async Task<bool> GetNextBatchAsync()
        {
            if (string.IsNullOrEmpty(_people.Links.Prev())) return true;
            if (string.IsNullOrEmpty(_people.Links.Next())) return false;
            _people = await _peopleClient.GetAsync<List<Person>>(_people.Links.Next());
            return true;
        }

        public async Task ProcessBatchAsync(Stats stats)
        {
            foreach (var person in _people.Data)
            {
                if (_existing.TryGetValue(person.Id, out var mapping))
                {
                    stats.Skipped++;
                    continue;
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
                    stats.Success++;
                }
                catch (OrbitApiException orbitEx)
                {
                    mapping.Error = orbitEx.Message;
                    _log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                        mapping.Error);
                    stats.Failed++;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                    mapping.Error = ex.ToString();
                    stats.Failed++;
                }

                _logDb.Mappings.Add(mapping);
            }
            await _logDb.SaveChangesAsync();
        }
    }
}