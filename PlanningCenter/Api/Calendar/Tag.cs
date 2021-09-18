using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class Tag : EntityBase
    {
        public string Color { get; set; }
        public string CreatedAt { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string UpdatedAt { get; set; }
    }
}