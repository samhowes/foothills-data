namespace PlanningCenter.Api.Services
{
    public class Tag : EntityBase
    {
        public string Name { get; set; }
        public string TagGroupId { get; set; }
        public TagGroup TagGroup { get; set; }
    }
}