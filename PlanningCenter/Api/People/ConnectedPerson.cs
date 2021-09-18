using JsonApi;

namespace PlanningCenter.Api.People
{
    public class ConnectedPerson : EntityBase
    {
        public string GivenName { get; set; }
        public string FirstName { get; set; }
        public string Nickname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}