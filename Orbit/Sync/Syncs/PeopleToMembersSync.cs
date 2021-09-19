using System.Threading.Tasks;
using JsonApi;
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

        public PeopleToMembersSync(PeopleClient peopleClient)
        {
            _peopleClient = peopleClient;
        }

        public string To => "Members";

        public Task<PlanningCenterCursor<Person>?> InitializeAsync(SyncContext context)
        {
            _context = context;
            var url = context.NextUrl ?? UrlUtil.MakeUrl("people",
                ("order", "-created_at"));
            return Task.FromResult(new PlanningCenterCursor<Person>(_peopleClient, url))!;
        }

        public async Task ProcessItemAsync(Person person)
        {
            // await CreateMemberAsync(person);
            _context.BatchProgress.Success++;
        }
    }
}