namespace PlanningCenter.Api.Groups
{
    public class Attendance : EntityBase
    {
        public string Attended { get; set; }
        public string Role { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string EventId { get; set; }
        public Event Event { get; set; }
    }
}