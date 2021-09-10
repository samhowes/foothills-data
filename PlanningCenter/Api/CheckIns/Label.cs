namespace PlanningCenter.Api.CheckIns
{
    public class Label : EntityBase
    {
        public string Name { get; set; }
        public string Xml { get; set; }
        public string PrintsFor { get; set; }
        public string Roll { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}