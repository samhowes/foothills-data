namespace PlanningCenter.Api.Services
{
    public class Contributor : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ContributableAction { get; set; }
        public string ContributableCategory { get; set; }
        public string ContributableType { get; set; }
        public string FullName { get; set; }
        public string PhotoThumbnailUrl { get; set; }
    }
}