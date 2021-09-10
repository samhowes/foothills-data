namespace PlanningCenter.Api.Services
{
    public class SignupSheet : EntityBase
    {
        public string SortDate { get; set; }
        public string GroupKey { get; set; }
        public string TeamName { get; set; }
        public string DisplayTimes { get; set; }
        public string PositionName { get; set; }
        public string Title { get; set; }
        public string SortIndex { get; set; }
    }
}