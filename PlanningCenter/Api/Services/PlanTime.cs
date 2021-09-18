using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class PlanTime : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Name { get; set; }
        public string TimeType { get; set; }
        public string Recorded { get; set; }
        public string TeamReminders { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string LiveStartsAt { get; set; }
        public string LiveEndsAt { get; set; }
    }
}