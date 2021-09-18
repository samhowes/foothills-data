using JsonApi;

namespace PlanningCenter.Api.Groups
{
    public class Tag : EntityBase
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string TagGroupId { get; set; }
        public TagGroup TagGroup { get; set; }
    }
}