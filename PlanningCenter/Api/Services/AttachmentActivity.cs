using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class AttachmentActivity : EntityBase
    {
        public string Date { get; set; }
        public string AttachmentUrl { get; set; }
        public string ActivityType { get; set; }
        public string AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }
}