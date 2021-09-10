namespace PlanningCenter.Api.People
{
    public class Household : EntityBase
    {
        public string Name { get; set; }
        public string MemberCount { get; set; }
        public string PrimaryContactName { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Avatar { get; set; }
        public string PrimaryContactId { get; set; }
        public Person PrimaryContact { get; set; }
    }
}