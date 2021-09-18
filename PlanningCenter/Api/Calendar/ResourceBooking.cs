using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class ResourceBooking : EntityBase
    {
        public string CreatedAt { get; set; }
        public string EndsAt { get; set; }
        public string StartsAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Quantity { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
        public string EventInstanceId { get; set; }
        public EventInstance EventInstance { get; set; }
        public string ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}