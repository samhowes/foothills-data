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
    }
}