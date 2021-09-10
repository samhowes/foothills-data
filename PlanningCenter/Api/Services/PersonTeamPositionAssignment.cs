namespace PlanningCenter.Api.Services
{
    public class PersonTeamPositionAssignment : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string SchedulePreference { get; set; }
        public string PreferredWeeks { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string TeamPositionId { get; set; }
        public TeamPosition TeamPosition { get; set; }
    }
}