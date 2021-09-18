using JsonApi;

namespace PlanningCenter.Api.People
{
    public class NoteCategorySubscription : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string NoteCategoryId { get; set; }
        public NoteCategory NoteCategory { get; set; }
    }
}