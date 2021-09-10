namespace PlanningCenter.Api.Services
{
    public class SignupSheetMetadata : EntityBase
    {
        public string Conflicts { get; set; }
        public string TimeType { get; set; }
        public string TimeName { get; set; }
        public string StartsAt { get; set; }
        public string PlanTimeId { get; set; }
        public PlanTime PlanTime { get; set; }
    }
}