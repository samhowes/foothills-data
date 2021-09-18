using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class SplitTeamRehearsalAssignment : EntityBase
    {
        public string ScheduleSpecialServiceTimes { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
    }
}