using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Workflow : EntityBase
    {
        public string Name { get; set; }
        public string MyReadyCardCount { get; set; }
        public string TotalReadyCardCount { get; set; }
        public string CompletedCardCount { get; set; }
        public string TotalCardsCount { get; set; }
        public string TotalReadyAndSnoozedCardCount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string DeletedAt { get; set; }
        public string CampusId { get; set; }
        public Campus Campus { get; set; }
        public string WorkflowCategoryId { get; set; }
        public WorkflowCategory WorkflowCategory { get; set; }
    }
}