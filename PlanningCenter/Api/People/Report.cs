using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Report : EntityBase
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}