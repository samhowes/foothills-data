namespace PlanningCenter.Api.Calendar
{
    public class Conflict : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Note { get; set; }
        public string ResolvedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}