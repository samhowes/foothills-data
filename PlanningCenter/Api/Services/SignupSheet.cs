using JsonApi;

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
        public string PlanId { get; set; }
        public Plan Plan { get; set; }
        public string TeamPositionId { get; set; }
        public TeamPosition TeamPosition { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
    }
}