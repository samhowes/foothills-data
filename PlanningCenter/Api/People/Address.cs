using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Address : EntityBase
    {
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Street { get; set; }
        public string Location { get; set; }
        public string Primary { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}