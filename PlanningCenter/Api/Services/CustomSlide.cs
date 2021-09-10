namespace PlanningCenter.Api.Services
{
    public class CustomSlide : EntityBase
    {
        public string Body { get; set; }
        public string Label { get; set; }
        public string Order { get; set; }
        public string Enabled { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
        public string AttachmentId { get; set; }
        public Attachment Attachment { get; set; }
    }
}