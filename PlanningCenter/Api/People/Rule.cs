using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Rule : EntityBase
    {
        public string Subset { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}