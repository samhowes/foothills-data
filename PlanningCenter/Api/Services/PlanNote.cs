namespace PlanningCenter.Api.Services
{
    public class PlanNote : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CategoryName { get; set; }
        public string Content { get; set; }
        public string PlanNoteCategoryId { get; set; }
        public PlanNoteCategory PlanNoteCategory { get; set; }
    }
}