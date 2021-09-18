using JsonApi;

namespace PlanningCenter.Api.Webhooks
{
    public class Delivery : EntityBase
    {
        public string Status { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestBody { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Timing { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
    }
}