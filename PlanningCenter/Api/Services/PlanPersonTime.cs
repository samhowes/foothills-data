using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class PlanPersonTime : EntityBase
    {
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PlanTimeId { get; set; }
        public PlanTime PlanTime { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string PlanPersonId { get; set; }
        public PlanPerson PlanPerson { get; set; }
    }
}