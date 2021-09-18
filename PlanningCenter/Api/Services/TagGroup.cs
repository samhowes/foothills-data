using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class TagGroup : EntityBase
    {
        public string Name { get; set; }
        public string Required { get; set; }
        public string AllowMultipleSelections { get; set; }
        public string TagsFor { get; set; }
        public string ServiceTypeFolderName { get; set; }
    }
}