using JsonApi;
using Newtonsoft.Json;
using PlanningCenter.Api.CheckIns;

namespace PlanningCenter.Api.People
{
    public class Note : EntityBase, IHavePerson
    {
        [JsonProperty("note")]
        public string Value { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string DisplayDate { get; set; }
        public NoteCategory NoteCategory { get; set; }
        public Organization Organization { get; set; }
        public Person? Person { get; set; }
        IPerson? IHavePerson.Person => Person;
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
    }
}