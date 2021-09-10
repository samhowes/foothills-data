namespace PlanningCenter.Api.Services
{
    public class ItemNote : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Content { get; set; }
        public string CategoryName { get; set; }
    }
}