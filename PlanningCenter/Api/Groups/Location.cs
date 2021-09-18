using JsonApi;

namespace PlanningCenter.Api.Groups
{
    public class Location : EntityBase
    {
        public string DisplayPreference { get; set; }
        public string FullFormattedAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Name { get; set; }
        public string Radius { get; set; }
        public string Strategy { get; set; }
    }
}