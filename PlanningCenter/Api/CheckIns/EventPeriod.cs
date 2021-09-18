using System;
using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class EventPeriod : EntityBase
    {
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string RegularCount { get; set; }
        public string GuestCount { get; set; }
        public string VolunteerCount { get; set; }
        public string Note { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
    }
}