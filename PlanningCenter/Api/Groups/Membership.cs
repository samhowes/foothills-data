namespace PlanningCenter.Api.Groups
{
    public class Membership : EntityBase
    {
        public string AccountCenterIdentifier { get; set; }
        public string AvatarUrl { get; set; }
        public string ColorIdentifier { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string JoinedAt { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string GroupId { get; set; }
        public Group Group { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
    }
}