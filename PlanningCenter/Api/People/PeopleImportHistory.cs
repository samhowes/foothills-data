using JsonApi;

namespace PlanningCenter.Api.People
{
    public class PeopleImportHistory : EntityBase
    {
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ConflictingChanges { get; set; }
        public string Kind { get; set; }
    }
}