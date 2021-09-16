using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonApi;
using JsonApiSerializer.JsonApi;
using Microsoft.EntityFrameworkCore;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.CheckIns;

namespace Sync
{
    public static class Constants
    {
        public const string PlanningCenterSource = "planningcenter";
    }
    
    public class PeopleToMembersSync : Sync<Person>
    {
        private readonly PeopleClient _peopleClient;
        private Dictionary<string,Mapping> _existing = null!;
        private DocumentRoot<List<Person>> _people = null!;
        
        public PeopleToMembersSync(PeopleClient peopleClient, SyncDeps deps)
            : base(deps, peopleClient)
        {
            _peopleClient = peopleClient;
        }

        public override string To => "Members";

        public override async Task<DocumentRoot<List<Person>>> GetInitialDataAsync(string? nextUrl)
        {
            _people = await _peopleClient.GetAsync<List<Person>>(nextUrl ?? "people");
            _existing = await Deps.LogDb.Mappings
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
                var created = await Deps.OrbitClient.PostAsync<Member>("members", member);
                mapping.OrbitId = created.Data.Id;
                progress.Success++;
            }
            catch (ApiException orbitEx)
            {
                mapping.Error = orbitEx.Message;
                Deps.Log.Error("Orbit api error for PlanningCenterId {PlanningCenterId}: {ApiError}", person.Id,
                    mapping.Error);
                progress.Failed++;
            }
            catch (Exception ex)
            {
                Deps.Log.Error(ex, "Unexpected error for PlanningCenterId {PlanningCenterId}", person.Id);
                mapping.Error = ex.ToString();
                progress.Failed++;
            }

            Deps.LogDb.Mappings.Add(mapping);
        }
    }
}