namespace PlanningCenter.Api.Services
{
    public class Item : EntityBase
    {
        public string Title { get; set; }
        public string Sequence { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Length { get; set; }
        public string ItemType { get; set; }
        public string HtmlDetails { get; set; }
        public string ServicePosition { get; set; }
        public string Description { get; set; }
        public string KeyName { get; set; }
        public string CustomArrangementSequence { get; set; }
        public string CustomArrangementSequenceShort { get; set; }
        public string CustomArrangementSequenceFull { get; set; }
    }
}