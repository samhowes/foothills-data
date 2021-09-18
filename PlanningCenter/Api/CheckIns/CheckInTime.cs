using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class CheckInTime : EntityBase
    {
        public string Kind { get; set; }
        public string HasValidated { get; set; }
        public string Errors { get; set; }
        public string ServicesIntegrated { get; set; }
        public string EventTimeId { get; set; }
        public EventTime EventTime { get; set; }
        public string LocationId { get; set; }
        public Location Location { get; set; }
        public string CheckInId { get; set; }
        public CheckIn CheckIn { get; set; }
    }
}