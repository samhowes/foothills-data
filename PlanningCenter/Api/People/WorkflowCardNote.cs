namespace PlanningCenter.Api.People
{
    public class WorkflowCardNote : EntityBase
    {
        public string Note { get; set; }
        public string CreatedAt { get; set; }
        public string NoteCategoryId { get; set; }
        public NoteCategory NoteCategory { get; set; }
    }
}