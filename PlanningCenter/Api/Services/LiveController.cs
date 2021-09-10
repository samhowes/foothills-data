namespace PlanningCenter.Api.Services
{
    public class LiveController : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string FullName { get; set; }
        public string PhotoThumbnailUrl { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}