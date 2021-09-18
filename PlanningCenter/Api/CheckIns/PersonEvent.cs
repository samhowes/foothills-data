using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class PersonEvent : EntityBase
    {
        public string CheckInCount { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }
    }
}