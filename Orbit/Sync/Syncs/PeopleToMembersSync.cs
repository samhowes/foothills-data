using System;
using System.Threading.Tasks;
using JsonApi;
using Orbit.Api;
using Orbit.Api.Model;
using PlanningCenter.Api;
using PlanningCenter.Api.People;

namespace Sync
{
    public static class Constants
    {
        public const string PlanningCenterSource = "planningcenter";
    }

    public class PeopleToMembersSync : ISync<Person>
    {
        private readonly PeopleClient _peopleClient;
        private SyncContext _context = null!;
        private readonly OrbitSync _orbitSync;

        public PeopleToMembersSync(PeopleClient peopleClient, OrbitSync orbitSync, OrbitApiClient orbitClient)
        {
            _peopleClient = peopleClient;
            _orbitSync = orbitSync;
        }

        public string To => "Members";

        public Task<ApiCursor<Person>?> InitializeAsync(SyncContext context)
        {
            _context = context;
            var url = context.NextUrl ?? UrlUtil.MakeUrl("people",
                ("order", "-created_at"));
            return Task.FromResult(new ApiCursor<Person>(_peopleClient, url))!;
        }

        public async Task ProcessItemAsync(Person person)
        {
            var now = DateTime.Now.ToUniversalTime();
            try
            {
                var maybeCreated = await _orbitSync.CreateMemberAsync(person);
                if (maybeCreated == null)
                {
                    _context.BatchProgress.Failed++;
                    return;
                }
                
                if (maybeCreated.CreatedAt < now)
                {
                    _context.BatchProgress.Skipped++;
                    _context.BatchProgress.Complete = true;
                    return;

                }
            }
            catch (ApiErrorException ex)
            {
                if (ex.Type == ErrorTypeEnum.AlreadyTaken)
                {
                    
                }
                _context.BatchProgress.Failed++;
                return;
            }
            
            _context.BatchProgress.Success++;
        }
    }
}