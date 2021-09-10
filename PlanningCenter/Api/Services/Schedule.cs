namespace PlanningCenter.Api.Services
{
    public class Schedule
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
    }
}