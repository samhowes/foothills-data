using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class ResourceQuestion : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Kind { get; set; }
        public string UpdatedAt { get; set; }
        public string Choices { get; set; }
        public string Description { get; set; }
        public string MultipleSelect { get; set; }
        public string Optional { get; set; }
        public string Position { get; set; }
        public string Question { get; set; }
        public string ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}