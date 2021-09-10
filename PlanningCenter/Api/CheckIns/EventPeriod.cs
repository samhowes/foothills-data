namespace PlanningCenter.Api.CheckIns
{
    public class EventPeriod : EntityBase
    {
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string RegularCount { get; set; }
        public string GuestCount { get; set; }
        public string VolunteerCount { get; set; }
        public string Note { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
    }
}