using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class ServiceType : EntityBase
    {
        public string ArchivedAt { get; set; }
        public string CreatedAt { get; set; }
        public string DeletedAt { get; set; }
        public string Name { get; set; }
        public string Sequence { get; set; }
        public string UpdatedAt { get; set; }
        public string AttachmentTypesEnabled { get; set; }
        public string BackgroundCheckPermissions { get; set; }
        public string CommentPermissions { get; set; }
        public string CustomItemTypes { get; set; }
        public string Frequency { get; set; }
        public string LastPlanFrom { get; set; }
        public string Permissions { get; set; }
        public string StandardItemTypes { get; set; }
        public string ParentId { get; set; }
        public Folder Parent { get; set; }
    }
}