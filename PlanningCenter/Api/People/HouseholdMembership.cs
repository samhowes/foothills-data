using JsonApi;

namespace PlanningCenter.Api.People
{
    public class HouseholdMembership : EntityBase
    {
        public string PersonName { get; set; }
        public string Pending { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}