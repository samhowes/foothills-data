namespace PlanningCenter.Api.Services
{
    public class AvailableSignup : EntityBase
    {
        public string OrganizationName { get; set; }
        public string PlanningCenterUrl { get; set; }
        public string ServiceTypeName { get; set; }
        public string SignupsAvailable { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}