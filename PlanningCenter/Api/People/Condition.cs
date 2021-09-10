namespace PlanningCenter.Api.People
{
    public class Condition : EntityBase
    {
        public string Application { get; set; }
        public string DefinitionClass { get; set; }
        public string Comparison { get; set; }
        public string Settings { get; set; }
        public string DefinitionIdentifier { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedById { get; set; }
        public Person CreatedBy { get; set; }
    }
}