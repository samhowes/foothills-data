using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class Schedule : EntityBase
    {
        public string SortDate { get; set; }
        public string Dates { get; set; }
        public string DeclineReason { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationTimeZone { get; set; }
        public string OrganizationTwentyFourHourTime { get; set; }
        public string PersonName { get; set; }
        public string PositionDisplayTimes { get; set; }
        public string RespondsToName { get; set; }
        public string ServiceTypeName { get; set; }
        public string ShortDates { get; set; }
        public string Status { get; set; }
        public string TeamName { get; set; }
        public string TeamPositionName { get; set; }
        public string CanAcceptPartial { get; set; }
        public string CanAcceptPartialOneTime { get; set; }
        public string PlanVisible { get; set; }
        public string PlanVisibleToMe { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string PlanPersonId { get; set; }
        public PlanPerson PlanPerson { get; set; }
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
        public string RespondsToPersonId { get; set; }
        public Person RespondsToPerson { get; set; }
    }
}