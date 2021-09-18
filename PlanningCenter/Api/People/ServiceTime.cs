using JsonApi;

namespace PlanningCenter.Api.People
{
    public class ServiceTime : EntityBase
    {
        public string StartTime { get; set; }
        public string Day { get; set; }
        public string Description { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string CampusId { get; set; }
        public Campus Campus { get; set; }
    }
}