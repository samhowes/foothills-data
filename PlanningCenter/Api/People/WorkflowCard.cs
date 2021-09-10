namespace PlanningCenter.Api.People
{
    public class WorkflowCard
    {
        public string SnoozeUntil { get; set; }
        public string Overdue { get; set; }
        public string Stage { get; set; }
        public string CalculatedDueAtInDaysAgo { get; set; }
        public string StickyAssignment { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CompletedAt { get; set; }
        public string FlaggedForNotificationAt { get; set; }
        public string RemovedAt { get; set; }
        public string MovedToStepAt { get; set; }
    }
}