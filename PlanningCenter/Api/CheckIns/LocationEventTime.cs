namespace PlanningCenter.Api.CheckIns
{
    public class LocationEventTime : EntityBase
    {
        public string RegularCount { get; set; }
        public string GuestCount { get; set; }
        public string VolunteerCount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}