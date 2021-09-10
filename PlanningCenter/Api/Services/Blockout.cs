namespace PlanningCenter.Api.Services
{
    public class Blockout : EntityBase
    {
        public string Description { get; set; }
        public string GroupIdentifier { get; set; }
        public string OrganizationName { get; set; }
        public string Reason { get; set; }
        public string RepeatFrequency { get; set; }
        public string RepeatInterval { get; set; }
        public string RepeatPeriod { get; set; }
        public string Settings { get; set; }
        public string TimeZone { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string RepeatUntil { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string Share { get; set; }
    }
}