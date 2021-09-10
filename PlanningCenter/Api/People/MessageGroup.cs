namespace PlanningCenter.Api.People
{
    public class MessageGroup
    {
        public string Uuid { get; set; }
        public string MessageType { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string MessageCount { get; set; }
        public string SystemMessage { get; set; }
        public string CreatedAt { get; set; }
    }
}