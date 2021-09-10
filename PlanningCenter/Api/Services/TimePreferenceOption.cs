namespace PlanningCenter.Api.Services
{
    public class TimePreferenceOption : EntityBase
    {
        public string DayOfWeek { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Description { get; set; }
        public string SortIndex { get; set; }
        public string TimeType { get; set; }
        public string MinuteOfDay { get; set; }
        public string StartsAt { get; set; }
    }
}