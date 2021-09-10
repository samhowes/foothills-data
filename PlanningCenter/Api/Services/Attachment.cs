namespace PlanningCenter.Api.Services
{
    public class Attachment : EntityBase
    {
        public string CreatedAt { get; set; }
        public string PageOrder { get; set; }
        public string UpdatedAt { get; set; }
        public string Filename { get; set; }
        public string FileSize { get; set; }
        public string LicensesPurchased { get; set; }
        public string LicensesRemaining { get; set; }
        public string LicensesUsed { get; set; }
        public string ContentType { get; set; }
        public string DisplayName { get; set; }
        public string Filetype { get; set; }
        public string LinkedUrl { get; set; }
        public string PcoType { get; set; }
        public string RemoteLink { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Url { get; set; }
        public string AllowMp3Download { get; set; }
        public string WebStreamable { get; set; }
        public string Downloadable { get; set; }
        public string Transposable { get; set; }
        public string Streamable { get; set; }
        public string HasPreview { get; set; }
        public string AttachableId { get; set; }
        public Plan Attachable { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
        public string UpdatedById { get; set; }
        public Person UpdatedBy { get; set; }
        public string AdministratorId { get; set; }
        public Person Administrator { get; set; }
    }
}