namespace PlanningCenter.Api.Services
{
    public class Person : EntityBase
    {
        public string PhotoUrl { get; set; }
        public string PhotoThumbnailUrl { get; set; }
        public string PreferredApp { get; set; }
        public string AssignedToRehearsalTeam { get; set; }
        public string ArchivedAt { get; set; }
        public string CreatedAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NamePrefix { get; set; }
        public string NameSuffix { get; set; }
        public string UpdatedAt { get; set; }
        public string FacebookId { get; set; }
        public string LegacyId { get; set; }
        public string FullName { get; set; }
        public string MaxPermissions { get; set; }
        public string Permissions { get; set; }
        public string Status { get; set; }
        public string Anniversary { get; set; }
        public string Birthdate { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Nickname { get; set; }
        public string AccessMediaAttachments { get; set; }
        public string AccessPlanAttachments { get; set; }
        public string AccessSongAttachments { get; set; }
        public string Archived { get; set; }
        public string SiteAdministrator { get; set; }
        public string LoggedInAt { get; set; }
        public string Notes { get; set; }
        public string PassedBackgroundCheck { get; set; }
        public string IcalCode { get; set; }
        public string PraiseChartsEnabled { get; set; }
        public string MeTab { get; set; }
        public string PlansTab { get; set; }
        public string SongsTab { get; set; }
        public string MediaTab { get; set; }
        public string PeopleTab { get; set; }
        public string CanEditAllPeople { get; set; }
        public string CanViewAllPeople { get; set; }
        public string Onboardings { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public Person UpdatedBy { get; set; }
        public string CurrentFolderId { get; set; }
        public Folder CurrentFolder { get; set; }
    }
}