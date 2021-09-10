namespace PlanningCenter.Api.Groups
{
    public class Resource : EntityBase
    {
        public string Description { get; set; }
        public string LastUpdated { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Visibility { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
    }
}