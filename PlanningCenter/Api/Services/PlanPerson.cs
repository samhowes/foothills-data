namespace PlanningCenter.Api.Services
{
    public class PlanPerson
    {
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Notes { get; set; }
        public string DeclineReason { get; set; }
        public string Name { get; set; }
        public string NotificationChangedByName { get; set; }
        public string NotificationSenderName { get; set; }
        public string TeamPositionName { get; set; }
        public string PhotoThumbnail { get; set; }
        public string StatusUpdatedAt { get; set; }
        public string NotificationChangedAt { get; set; }
        public string NotificationPreparedAt { get; set; }
        public string NotificationReadAt { get; set; }
        public string NotificationSentAt { get; set; }
        public string PrepareNotification { get; set; }
        public string CanAcceptPartial { get; set; }
    }
}