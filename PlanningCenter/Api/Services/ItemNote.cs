namespace PlanningCenter.Api.Services
{
    public class ItemNote : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Content { get; set; }
        public string CategoryName { get; set; }
        public string ItemNoteCategoryId { get; set; }
        public ItemNoteCategory ItemNoteCategory { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
    }
}