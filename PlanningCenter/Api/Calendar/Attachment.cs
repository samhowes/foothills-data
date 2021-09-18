using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class Attachment : EntityBase
    {
        public string ContentType { get; set; }
        public string CreatedAt { get; set; }
        public string Description { get; set; }
        public string FileSize { get; set; }
        public string Name { get; set; }
        public string UpdatedAt { get; set; }
        public string Url { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
    }
}