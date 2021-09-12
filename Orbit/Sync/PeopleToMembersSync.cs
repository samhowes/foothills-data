using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonApi;
using Microsoft.EntityFrameworkCore;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public class PeopleToMembersSync : ISync
    {
        private readonly SyncDependencies _deps;
        private Dictionary<string,Mapping> _existing;
        private Response<List<Resource<Person>>> _people;


        public PeopleToMembersSync(SyncDependencies deps)
        {
            _deps = deps;
        }

        public string From => "People";
        public string To => "Members";

        public async Task<Response> GetInitialDataAsync()
        {
            _people = await _deps.PeopleClient.GetAsync<Person>("people");
            _existing = await _deps.LogDb.Mappings.ToDictionaryAsync(m => m.PlanningCenterId);
            return _people;
        }

        public async Task<bool> GetNextBatchAsync()
        {
            if (string.IsNullOrEmpty(_people.Links.Prev)) return true;
            if (string.IsNullOrEmpty(_people.Links.Next)) return false;
            _people = await _deps.PeopleClient.GetAsync<Person>(_people.Links.Next);
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
                if (person.Attributes.Child == "true")
                    tags.Add("child");

                var member = new UpsertMember()
                {
                    Birthday = person.Attributes.Birthdate,
                    Name = person.Attributes.Name,
                    Slug = person.Id,
                    TagsToAdd = string.Join(",", tags),
                    Identity = new Identity(source: "planningcenter")
                    {
                        Email = $"{person.Id}@foothillsuu.org",
                        Name = person.Attributes.Name,
                        Uid = person.Id,
                        Url = person.Links.Self,
                    }
                };

                try
                {
                    var created = await _deps.OrbitClient.PostAsync<Member>($"{_deps.WorkspaceSlug}/members", member);
                    mapping.OrbitId = created.Data.Id;
                    stats.Success++;
                }
                catch (OrbitApiException orbitEx)
                {
                    mapping.Error = orbitEx.Message;
                    _deps.Log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                        mapping.Error);
                    stats.Failed++;
                }
                catch (Exception ex)
                {
                    _deps.Log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                    mapping.Error = ex.ToString();
                    stats.Failed++;
                }

                _deps.LogDb.Mappings.Add(mapping);
            }
            await _deps.LogDb.SaveChangesAsync();
        }
    }
}