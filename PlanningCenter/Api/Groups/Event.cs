namespace PlanningCenter.Api.Groups
{
    public class Event : EntityBase
    {
        public string AttendanceRequestsEnabled { get; set; }
        public string AutomatedReminderEnabled { get; set; }
        public string Canceled { get; set; }
        public string CanceledAt { get; set; }
        public string Description { get; set; }
        public string EndsAt { get; set; }
        public string LocationTypePreference { get; set; }
        public string MultiDay { get; set; }
        public string Name { get; set; }
        public string RemindersSent { get; set; }
        public string RemindersSentAt { get; set; }
        public string Repeating { get; set; }
        public string StartsAt { get; set; }
        public string VirtualLocationUrl { get; set; }
    }
}