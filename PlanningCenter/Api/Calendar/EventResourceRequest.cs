using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class EventResourceRequest : EntityBase
    {
        public string ApprovalSent { get; set; }
        public string ApprovalStatus { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Notes { get; set; }
        public string Quantity { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
        public string ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string EventResourceRequestId { get; set; }
        public EventResourceRequest Parent { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public Person UpdatedBy { get; set; }
        public string RoomSetupId { get; set; }
        public RoomSetup RoomSetup { get; set; }
    }
}