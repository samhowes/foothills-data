namespace PlanningCenter.Api.Services
{
    public class NeededPosition : EntityBase
    {
        public string Quantity { get; set; }
        public string TeamPositionName { get; set; }
        public string ScheduledTo { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string TimeId { get; set; }
        public PlanTime Time { get; set; }
        public string TimePreferenceOptionId { get; set; }
        public TimePreferenceOption TimePreferenceOption { get; set; }
    }
}