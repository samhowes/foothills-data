using JsonApi;

namespace PlanningCenter.Api.People
{
    public class WorkflowShare : EntityBase
    {
        public string Group { get; set; }
        public string Permission { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
    }
}