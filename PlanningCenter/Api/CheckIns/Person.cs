using JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class Person : EntityBase, IPerson
    {
        public string Addresses { get; set; }
        public string EmailAddresses { get; set; }
        public string PhoneNumbers { get; set; }
        public string AvatarUrl { get; set; }
        public string NamePrefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string NameSuffix { get; set; }
        public string Birthdate { get; set; }
        public string Grade { get; set; }
        public string Gender { get; set; }
        public string MedicalNotes { get; set; }
        public string Child { get; set; }
        public string Permission { get; set; }
        public string Headcounter { get; set; }
        public string CheckInCount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string DemographicAvatarUrl { get; set; }
        public string Name { get; set; }
    }
}