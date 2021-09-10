namespace PlanningCenter.Api.Services
{
    public class Organization : EntityBase
    {
        public string Ccli { get; set; }
        public string CreatedAt { get; set; }
        public string DateFormat { get; set; }
        public string MusicStandEnabled { get; set; }
        public string Name { get; set; }
        public string ProjectorEnabled { get; set; }
        public string TimeZone { get; set; }
        public string TwentyFourHourTime { get; set; }
        public string UpdatedAt { get; set; }
        public string OwnerName { get; set; }
        public string RequiredToSetDownloadPermission { get; set; }
        public string Secret { get; set; }
        public string AllowMp3Download { get; set; }
        public string CalendarStartsOnSunday { get; set; }
        public string CcliConnected { get; set; }
        public string CcliReportingEnabled { get; set; }
        public string ExtraFileStorageAllowed { get; set; }
        public string FileStorageExceeded { get; set; }
        public string FileStorageSize { get; set; }
        public string FileStorageSizeUsed { get; set; }
        public string FileStorageExtraEnabled { get; set; }
        public string RehearsalMixEnabled { get; set; }
        public string LegacyId { get; set; }
        public string FileStorageExtraCharges { get; set; }
        public string PeopleAllowed { get; set; }
        public string PeopleRemaining { get; set; }
        public string Beta { get; set; }
    }
}