namespace PlanningCenter.Api.Calendar
{
    public class EventInstance
    {
        public string AllDayEvent { get; set; }
        public string CreatedAt { get; set; }
        public string EndsAt { get; set; }
        public string Location { get; set; }
        public string Recurrence { get; set; }
        public string RecurrenceDescription { get; set; }
        public string StartsAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}