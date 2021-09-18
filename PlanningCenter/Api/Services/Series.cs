using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class Series : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ArtworkFileName { get; set; }
        public string ArtworkContentType { get; set; }
        public string ArtworkFileSize { get; set; }
        public string Title { get; set; }
        public string ArtworkForDashboard { get; set; }
        public string ArtworkForMobile { get; set; }
        public string ArtworkForPlan { get; set; }
        public string ArtworkOriginal { get; set; }
    }
}