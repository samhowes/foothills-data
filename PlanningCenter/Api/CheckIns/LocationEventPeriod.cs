using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class LocationEventPeriod : EntityBase
    {
        public string RegularCount { get; set; }
        public string GuestCount { get; set; }
        public string VolunteerCount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}