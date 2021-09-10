namespace PlanningCenter.Api.People
{
    public class NoteCategory : EntityBase
    {
        public string Name { get; set; }
        public string Locked { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}