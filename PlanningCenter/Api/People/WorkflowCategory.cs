using JsonApi;

namespace PlanningCenter.Api.People
{
    public class WorkflowCategory : EntityBase
    {
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}