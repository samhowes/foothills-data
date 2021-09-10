namespace PlanningCenter.Api.People
{
    public class FieldOption : EntityBase
    {
        public string Value { get; set; }
        public string Sequence { get; set; }
        public string FieldDefinitionId { get; set; }
        public FieldDefinition FieldDefinition { get; set; }
    }
}