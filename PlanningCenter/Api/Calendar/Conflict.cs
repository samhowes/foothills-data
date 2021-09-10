namespace PlanningCenter.Api.Calendar
{
    public class Conflict : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Note { get; set; }
        public string ResolvedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string ResolvedById { get; set; }
        public Person ResolvedBy { get; set; }
        public string WinnerId { get; set; }
        public Event Winner { get; set; }
    }
}