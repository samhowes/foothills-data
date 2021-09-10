namespace PlanningCenter.Api.Calendar
{
    public class EventTime
    {
        public string EndsAt { get; set; }
        public string StartsAt { get; set; }
        public string Name { get; set; }
        public string VisibleOnKiosks { get; set; }
        public string VisibleOnWidgetAndIcal { get; set; }
    }
}