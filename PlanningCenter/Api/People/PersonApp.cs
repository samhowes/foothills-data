using JsonApi;

namespace PlanningCenter.Api.People
{
    public class PersonApp : EntityBase
    {
        public string AllowPcoLogin { get; set; }
        public string PeoplePermissions { get; set; }
        public string AppId { get; set; }
        public App App { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}