using JsonApi;

namespace PlanningCenter.Api.People
{
    public class FieldDatum : EntityBase
    {
        public string Value { get; set; }
        public string File { get; set; }
        public string FileSize { get; set; }
        public string FileContentType { get; set; }
        public string FileName { get; set; }
        public string FieldDefinitionId { get; set; }
        public FieldDefinition FieldDefinition { get; set; }
        public string CustomizableId { get; set; }
        public Customizable Customizable { get; set; }
    }
}