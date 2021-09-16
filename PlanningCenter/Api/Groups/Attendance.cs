namespace PlanningCenter.Api.Groups
{
    public class Attendance : EntityBase
    {
        public bool Attended { get; set; }
        public string Role { get; set; }
        public Person Person { get; set; }
        public Event Event { get; set; }
    }
}