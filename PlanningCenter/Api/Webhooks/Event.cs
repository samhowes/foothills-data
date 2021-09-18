using JsonApi;

namespace PlanningCenter.Api.Webhooks
{
    public class Event : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Uuid { get; set; }
        public string Payload { get; set; }
        public string SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
    }
}