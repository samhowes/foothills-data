namespace PlanningCenter.Api.People
{
    public class WorkflowStep
    {
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Sequence { get; set; }
        public string Description { get; set; }
        public string AutoSnoozeDays { get; set; }
        public string AutoSnoozeValue { get; set; }
        public string AutoSnoozeInterval { get; set; }
        public string ExpectedResponseTimeInDays { get; set; }
        public string MyReadyCardCount { get; set; }
        public string TotalReadyCardCount { get; set; }
        public string DefaultAssigneeId { get; set; }
    }
}