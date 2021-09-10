using Newtonsoft.Json;

namespace PlanningCenter.Api.People
{
    public class Note : EntityBase
    {
        [JsonProperty("note")]
        public string Value { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string DisplayDate { get; set; }
        public string NoteCategoryId { get; set; }
        public NoteCategory NoteCategory { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
    }
}