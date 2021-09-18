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
            
        }
    }
}