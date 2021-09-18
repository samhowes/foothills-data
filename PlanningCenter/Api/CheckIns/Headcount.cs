using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class Headcount : EntityBase
    {
        public string Total { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }
        public string EventTimeId { get; set; }
        public EventTime EventTime { get; set; }
        public string AttendanceTypeId { get; set; }
        public AttendanceType AttendanceType { get; set; }
    }
}