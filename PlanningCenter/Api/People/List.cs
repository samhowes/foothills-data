using JsonApi;

namespace PlanningCenter.Api.People
{
    public class PeopleList : EntityBase
    {
        public string Name { get; set; }
        public string AutoRefresh { get; set; }
        public string Status { get; set; }
        public string HasInactiveResults { get; set; }
        public string IncludeInactive { get; set; }
        public string Returns { get; set; }
        public string ReturnOriginalIfNone { get; set; }
        public string Subset { get; set; }
        public string AutomationsActive { get; set; }
        public string AutomationsCount { get; set; }
        public string Description { get; set; }
        public string Invalid { get; set; }
        public string NameOrDescription { get; set; }
        public string RecentlyViewed { get; set; }
        public string RefreshedAt { get; set; }
        public string Starred { get; set; }
        public string TotalPeople { get; set; }
        public string BatchCompletedAt { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}