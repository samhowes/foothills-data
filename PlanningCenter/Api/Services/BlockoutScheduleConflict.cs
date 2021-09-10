namespace PlanningCenter.Api.Services
{
    public class BlockoutScheduleConflict : EntityBase
    {
        public string Dates { get; set; }
        public string OrganizationName { get; set; }
        public string PersonAvatar { get; set; }
        public string PersonName { get; set; }
        public string PositionDisplayTimes { get; set; }
        public string ServiceTypeName { get; set; }
        public string ShortDates { get; set; }
        public string Status { get; set; }
        public string TeamName { get; set; }
        public string TeamPositionName { get; set; }
        public string SortDate { get; set; }
        public string CanAcceptPartial { get; set; }
    }
}