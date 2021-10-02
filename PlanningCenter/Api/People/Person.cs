using System;
using JsonApi;

namespace PlanningCenter.Api.People
{
    public class Person : EntityBase, IPerson
    {
        public string GivenName { get; set; }
        public string FirstName { get; set; }
        public string Nickname { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Birthdate { get; set; }
        public string Anniversary { get; set; }
        public string Gender { get; set; }
        public string Grade { get; set; }
        public bool Child { get; set; }
        public string GraduationYear { get; set; }
        public string SiteAdministrator { get; set; }
        public string AccountingAdministrator { get; set; }
        public string PeoplePermissions { get; set; }
        public string? Membership { get; set; }
        public string InactivatedAt { get; set; }
        public string Status { get; set; }
        public string MedicalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string DemographicAvatarUrl { get; set; }
        public string DirectoryStatus { get; set; }
        public string PassedBackgroundCheck { get; set; }
        public string CanCreateForms { get; set; }
        public string SchoolType { get; set; }
        public string RemoteId { get; set; }
        public string PrimaryCampusId { get; set; }
        public PrimaryCampus PrimaryCampus { get; set; }
        public string OrbitWorkspace { get; set; }
    }
}