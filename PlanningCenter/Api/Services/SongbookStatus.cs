namespace PlanningCenter.Api.Services
{
    public class SongbookStatus : EntityBase
    {
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string StatusToken { get; set; }
        public string Url { get; set; }
    }
}