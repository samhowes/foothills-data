namespace PlanningCenter.Api.Services
{
    public class PublicView : EntityBase
    {
        public string SeriesAndPlanTitles { get; set; }
        public string ItemLengths { get; set; }
        public string ServiceTimes { get; set; }
        public string SongItems { get; set; }
        public string MediaItems { get; set; }
        public string RegularItems { get; set; }
        public string Headers { get; set; }
        public string Itunes { get; set; }
        public string Amazon { get; set; }
        public string Spotify { get; set; }
        public string Youtube { get; set; }
        public string Vimeo { get; set; }
    }
}