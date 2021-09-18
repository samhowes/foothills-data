using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class SongSchedule : EntityBase
    {
        public string ArrangementName { get; set; }
        public string KeyName { get; set; }
        public string PlanDates { get; set; }
        public string ServiceTypeName { get; set; }
        public string PlanSortDate { get; set; }
        public string ArrangementId { get; set; }
        public Arrangement Arrangement { get; set; }
        public string KeyId { get; set; }
        public Key Key { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
    }
}