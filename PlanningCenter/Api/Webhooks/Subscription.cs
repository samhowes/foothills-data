namespace PlanningCenter.Api.Webhooks
{
    public class Subscription
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Active { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AuthenticitySecret { get; set; }
        public string ApplicationId { get; set; }
    }
}