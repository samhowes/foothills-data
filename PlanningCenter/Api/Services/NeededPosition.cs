namespace PlanningCenter.Api.Services
{
    public class NeededPosition : EntityBase
    {
        public string Quantity { get; set; }
        public string TeamPositionName { get; set; }
        public string ScheduledTo { get; set; }
    }
}