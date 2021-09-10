namespace PlanningCenter.Api.Services
{
    public class PersonTeamPositionAssignment : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string SchedulePreference { get; set; }
        public string PreferredWeeks { get; set; }
    }
}