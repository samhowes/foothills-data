using JsonApi;

namespace PlanningCenter.Api.People
{
    public class SocialProfile : EntityBase
    {
        public string Site { get; set; }
        public string Url { get; set; }
        public string Verified { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}