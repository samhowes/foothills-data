using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Campus : EntityBase
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string TwentyFourHourTime { get; set; }
        public string DateFormat { get; set; }
        public string ChurchCenterEnabled { get; set; }
        public string ContactEmailAddress { get; set; }
        public string TimeZone { get; set; }
        public string GeolocationSetManually { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AvatarUrl { get; set; }
        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}