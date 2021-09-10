namespace PlanningCenter.Api.People
{
    public class FormFieldOption : EntityBase
    {
        public string Label { get; set; }
        public string Sequence { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string FormFieldId { get; set; }
        public FormField FormField { get; set; }
        public string OptionableId { get; set; }
        public Optionable Optionable { get; set; }
    }
}