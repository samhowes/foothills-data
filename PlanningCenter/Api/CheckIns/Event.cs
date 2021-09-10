namespace PlanningCenter.Api.CheckIns
{
    public class Event : EntityBase
    {
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string EnableServicesIntegration { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ArchivedAt { get; set; }
        public string IntegrationKey { get; set; }
        public string LocationTimesEnabled { get; set; }
        public string PreSelectEnabled { get; set; }
    }
}