namespace PlanningCenter.Api.People
{
    public class Email : EntityBase
    {
        public string Address { get; set; }
        public string Location { get; set; }
        public string Primary { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Blocked { get; set; }
    }
}