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
    }
}