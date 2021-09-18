using JsonApi;

namespace PlanningCenter.Api.People
{
    public class NoteCategoryShare : EntityBase
    {
        public string Group { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string NoteCategoryId { get; set; }
        public NoteCategory NoteCategory { get; set; }
    }
}