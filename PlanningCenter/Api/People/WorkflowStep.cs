using JsonApi;

namespace PlanningCenter.Api.People
{
    public class WorkflowStep : EntityBase
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
        public Person DefaultAssignee { get; set; }
        public string WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
    }
}