namespace PlanningCenter.Api.People
{
    public class PeopleImportConflict : EntityBase
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public string ConflictingChanges { get; set; }
        public string Ignore { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}