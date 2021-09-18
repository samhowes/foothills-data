using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class Event : EntityBase
    {
        public string ApprovalStatus { get; set; }
        public string CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string PercentApproved { get; set; }
        public string PercentRejected { get; set; }
        public string RegistrationUrl { get; set; }
        public string Summary { get; set; }
        public string UpdatedAt { get; set; }
        public string VisibleInChurchCenter { get; set; }
        public string OwnerId { get; set; }
        public Person Owner { get; set; }
    }
}