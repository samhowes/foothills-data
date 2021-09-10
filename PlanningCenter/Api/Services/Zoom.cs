namespace PlanningCenter.Api.Services
{
    public class Zoom : EntityBase
    {
        public string AspectRatio { get; set; }
        public string ZoomLevel { get; set; }
        public string XOffset { get; set; }
        public string YOffset { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string AttachableId { get; set; }
        public Attachment Attachable { get; set; }
        public string AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }
}