using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Form : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Active { get; set; }
        public string ArchivedAt { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string DeletedAt { get; set; }
        public string SubmissionCount { get; set; }
        public string PublicUrl { get; set; }
        public string RecentlyViewed { get; set; }
        public string Archived { get; set; }
        public string CampusId { get; set; }
        public Campus Campus { get; set; }
        public string FormCategoryId { get; set; }
        public FormCategory FormCategory { get; set; }
    }
}