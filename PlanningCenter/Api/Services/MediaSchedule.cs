namespace PlanningCenter.Api.Services
{
    public class MediaSchedule : EntityBase
    {
        public string PlanDates { get; set; }
        public string PlanShortDates { get; set; }
        public string ServiceTypeName { get; set; }
        public string PlanSortDate { get; set; }
    }
}