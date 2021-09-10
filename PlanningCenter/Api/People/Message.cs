namespace PlanningCenter.Api.People
{
    public class Message
    {
        public string Kind { get; set; }
        public string ToAddresses { get; set; }
        public string Subject { get; set; }
        public string File { get; set; }
        public string DeliveryStatus { get; set; }
        public string RejectReason { get; set; }
        public string CreatedAt { get; set; }
        public string SentAt { get; set; }
        public string BouncedAt { get; set; }
        public string RejectionNotificationSentAt { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string ReadAt { get; set; }
        public string AppName { get; set; }
    }
}