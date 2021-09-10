namespace PlanningCenter.Api.Calendar
{
    public class Organization : EntityBase
    {
        public string Name { get; set; }
        public string TimeZone { get; set; }
        public string TwentyFourHourTime { get; set; }
        public string DateFormat { get; set; }
    }
}