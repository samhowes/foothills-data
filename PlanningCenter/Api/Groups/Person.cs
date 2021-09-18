using JsonApi;

namespace PlanningCenter.Api.Groups
{
    public class Person : EntityBase
    {
        public string Addresses { get; set; }
        public string AvatarUrl { get; set; }
        public string CreatedAt { get; set; }
        public string EmailAddresses { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Permissions { get; set; }
        public string PhoneNumbers { get; set; }
    }
}