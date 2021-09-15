namespace PlanningCenter.Api.CheckIns
{
    public class AttendanceType : EntityBase
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public string? Limit { get; set; }
        public string? EventId { get; set; }
        public Event? Event { get; set; }
    }
}