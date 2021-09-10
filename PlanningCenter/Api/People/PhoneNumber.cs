namespace PlanningCenter.Api.People
{
    public class PhoneNumber : EntityBase
    {
        public string Number { get; set; }
        public string Carrier { get; set; }
        public string Location { get; set; }
        public string Primary { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}