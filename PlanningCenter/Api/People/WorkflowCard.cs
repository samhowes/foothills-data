using JsonApi;

namespace PlanningCenter.Api.People
{
    public class WorkflowCard : EntityBase
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
        public string AssigneeId { get; set; }
        public Assignee Assignee { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
        public string CurrentStepId { get; set; }
        public WorkflowStep CurrentStep { get; set; }
    }
}