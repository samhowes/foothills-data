namespace PlanningCenter.Api.People
{
    public class PersonMerger : EntityBase
    {
        public string CreatedAt { get; set; }
        public string PersonToKeepId { get; set; }
        public string PersonToRemoveId { get; set; }
    }
}