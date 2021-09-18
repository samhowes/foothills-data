using JsonApi;

namespace PlanningCenter.Api.Calendar
{
    public class Person : EntityBase
    {
        public string CreatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string UpdatedAt { get; set; }
        public string AvatarUrl { get; set; }
        public string Child { get; set; }
        public string ContactData { get; set; }
        public string Gender { get; set; }
        public string HasAccess { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }
        public string PendingRequestCount { get; set; }
        public string Permissions { get; set; }
        public string ResolvesConflicts { get; set; }
        public string SiteAdministrator { get; set; }
        public string Status { get; set; }
    }
}