using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class PlanPerson : EntityBase
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
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string ScheduledById { get; set; }
        public Person ScheduledBy { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
        public string RespondsToId { get; set; }
        public Person RespondsTo { get; set; }
    }
}