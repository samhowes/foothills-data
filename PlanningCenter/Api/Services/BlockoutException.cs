namespace PlanningCenter.Api.Services
{
    public class BlockoutException : EntityBase
    {
        public string Date { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string BlockoutId { get; set; }
        public Blockout Blockout { get; set; }
    }
}