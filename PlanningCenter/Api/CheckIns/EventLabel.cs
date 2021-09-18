using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class EventLabel : EntityBase
    {
        public string Quantity { get; set; }
        public string ForRegular { get; set; }
        public string ForGuest { get; set; }
        public string ForVolunteer { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}