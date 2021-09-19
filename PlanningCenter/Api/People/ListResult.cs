using JsonApi;

namespace PlanningCenter.Api.People
{
    public class ListResult : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string ListId { get; set; }
        public PeopleList List { get; set; }
    }
}