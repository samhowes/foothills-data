using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class Team : EntityBase
    {
        public string Name { get; set; }
        public string RehearsalTeam { get; set; }
        public string Sequence { get; set; }
        public string ScheduleTo { get; set; }
        public string DefaultStatus { get; set; }
        public string DefaultPrepareNotifications { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ArchivedAt { get; set; }
        public string AssignedDirectly { get; set; }
        public string SecureTeam { get; set; }
        public string LastPlanFrom { get; set; }
        public string StageColor { get; set; }
        public string StageVariant { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string DefaultRespondsToId { get; set; }
        public Person DefaultRespondsTo { get; set; }
    }
}