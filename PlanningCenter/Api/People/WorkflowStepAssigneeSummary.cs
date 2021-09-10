namespace PlanningCenter.Api.People
{
    public class WorkflowStepAssigneeSummary : EntityBase
    {
        public string ReadyCount { get; set; }
        public string SnoozedCount { get; set; }
    }
}