using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class Plan : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Title { get; set; }
        public string UpdatedAt { get; set; }
        public string Public { get; set; }
        public string SeriesTitle { get; set; }
        public string PlanNotesCount { get; set; }
        public string OtherTimeCount { get; set; }
        public string RehearsalTimeCount { get; set; }
        public string ServiceTimeCount { get; set; }
        public string PlanPeopleCount { get; set; }
        public string NeededPositionsCount { get; set; }
        public string ItemsCount { get; set; }
        public string TotalLength { get; set; }
        public string CanViewOrder { get; set; }
        public string MultiDay { get; set; }
        public string PrefersOrderView { get; set; }
        public string Rehearsable { get; set; }
        public string FilesExpireAt { get; set; }
        public string SortDate { get; set; }
        public string LastTimeAt { get; set; }
        public string Permissions { get; set; }
        public string Dates { get; set; }
        public string ShortDates { get; set; }
        public string PlanningCenterUrl { get; set; }
        public string RemindersDisabled { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string NextPlanId { get; set; }
        public Plan NextPlan { get; set; }
        public string PreviousPlanId { get; set; }
        public Plan PreviousPlan { get; set; }
        public string SeriesId { get; set; }
        public Series Series { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public Person UpdatedBy { get; set; }
        public string LinkedPublishingEpisodeId { get; set; }
        public LinkedPublishingEpisode LinkedPublishingEpisode { get; set; }
    }
}