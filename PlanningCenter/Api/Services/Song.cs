namespace PlanningCenter.Api.Services
{
    public class Song
    {
        public string Title { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Admin { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string Hidden { get; set; }
        public string Notes { get; set; }
        public string Themes { get; set; }
        public string LastScheduledShortDates { get; set; }
        public string LastScheduledAt { get; set; }
        public string CcliNumber { get; set; }
    }
}