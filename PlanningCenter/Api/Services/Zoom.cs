namespace PlanningCenter.Api.Services
{
    public class Zoom : EntityBase
    {
        public string AspectRatio { get; set; }
        public string ZoomLevel { get; set; }
        public string XOffset { get; set; }
        public string YOffset { get; set; }
    }
}