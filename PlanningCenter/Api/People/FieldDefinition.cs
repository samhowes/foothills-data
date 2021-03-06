using JsonApi;

namespace PlanningCenter.Api.People
{
    public class FieldDefinition : EntityBase
    {
        public string DataType { get; set; }
        public string Name { get; set; }
        public string Sequence { get; set; }
        public string Slug { get; set; }
        public string Config { get; set; }
        public string DeletedAt { get; set; }
        public string TabId { get; set; }
        public Tab Tab { get; set; }
    }
}