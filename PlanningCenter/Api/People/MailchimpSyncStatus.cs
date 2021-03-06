using JsonApi;

namespace PlanningCenter.Api.People
{
    public class MailchimpSyncStatus : EntityBase
    {
        public string Status { get; set; }
        public string Error { get; set; }
        public string Progress { get; set; }
        public string CompletedAt { get; set; }
        public string SegmentId { get; set; }
    }
}