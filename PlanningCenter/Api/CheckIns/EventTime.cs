namespace PlanningCenter.Api.CheckIns
{
    public class EventTime : EntityBase
    {
        public string TotalCount { get; set; }
        public string StartsAt { get; set; }
        public string ShowsAt { get; set; }
        public string HidesAt { get; set; }
        public string RegularCount { get; set; }
        public string GuestCount { get; set; }
        public string VolunteerCount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Name { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
        public string EventPeriodId { get; set; }
        public EventPeriod EventPeriod { get; set; }
    }
}