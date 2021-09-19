using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi;
using PlanningCenter.Api;
using PlanningCenter.Api.People;

namespace Sync
{
    public static class Constants
    {
        public const string PlanningCenterSource = "planningcenter";
    }
    
    public class PeopleToMembersSync : Sync<Person>
    {
        private readonly PeopleClient _peopleClient;
        private readonly FilesConfig _filesConfig;
        private StreamWriter _csv = null!;

        public PeopleToMembersSync(PeopleClient peopleClient, SyncDeps deps, FilesConfig filesConfig)
            : base(deps, peopleClient)
        {
            _peopleClient = peopleClient;
            _filesConfig = filesConfig;
        }

        public override string To => "Members";

        public override async Task<DocumentRoot<List<Person>>> GetInitialDataAsync(string? nextUrl)
        {
            await base.GetInitialDataAsync(nextUrl);
            var csv = new FileInfo(Path.Combine(_filesConfig.Root, "people.csv"));
            if (csv.Exists)
                csv.Delete();
            
            _csv = new StreamWriter(csv.OpenWrite());
            await _csv.WriteLineAsync("Name,Child,Membership,Status");
            var batch = await _peopleClient.GetAsync<List<Person>>(nextUrl ?? "people", 
                ("order", "-created_at"));
            return batch;
        }

        public override async Task ProcessBatchAsync(Progress progress, Person person)
        {
            await _csv.WriteLineAsync(string.Join(",", new[]
            {
                person.Name,
                person.Child.ToString(),
                person.Membership,
                person.Status,
            }));
            
            
            // await CreateMemberAsync(person);
            progress.Success++;
        }

        public override async Task AfterEachBatchAsync()
        {
            await base.AfterEachBatchAsync();
            await _csv.FlushAsync();
        }
    }
}