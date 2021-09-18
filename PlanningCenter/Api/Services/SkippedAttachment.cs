using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class SkippedAttachment : EntityBase
    {
        public string Skipped { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }
}