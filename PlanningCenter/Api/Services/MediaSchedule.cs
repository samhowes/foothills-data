namespace PlanningCenter.Api.Services
{
    public class MediaSchedule : EntityBase
    {
        public string PlanDates { get; set; }
        public string PlanShortDates { get; set; }
        public string ServiceTypeName { get; set; }
        public string PlanSortDate { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}