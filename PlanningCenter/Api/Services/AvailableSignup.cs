namespace PlanningCenter.Api.Services
{
    public class AvailableSignup : EntityBase
    {
        public string OrganizationName { get; set; }
        public string PlanningCenterUrl { get; set; }
        public string ServiceTypeName { get; set; }
        public string SignupsAvailable { get; set; }
    }
}