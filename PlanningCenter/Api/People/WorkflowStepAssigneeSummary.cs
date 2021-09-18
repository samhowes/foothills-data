using JsonApi;

namespace PlanningCenter.Api.People
{
    public class WorkflowStepAssigneeSummary : EntityBase
    {
        public string ReadyCount { get; set; }
        public string SnoozedCount { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string StepId { get; set; }
        public Step Step { get; set; }
    }
}