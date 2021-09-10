namespace PlanningCenter.Api.People
{
    public class FormField : EntityBase
    {
        public string FieldType { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Required { get; set; }
        public string Settings { get; set; }
        public string Sequence { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}