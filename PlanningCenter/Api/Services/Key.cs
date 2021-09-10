namespace PlanningCenter.Api.Services
{
    public class Key : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Name { get; set; }
        public string AlternateKeys { get; set; }
        public string EndingKey { get; set; }
        public string StartingKey { get; set; }
        public string StartingMinor { get; set; }
        public string EndingMinor { get; set; }
    }
}