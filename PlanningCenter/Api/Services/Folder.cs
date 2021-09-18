using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class Folder : EntityBase
    {
        public string CreatedAt { get; set; }
        public string Name { get; set; }
        public string UpdatedAt { get; set; }
        public string Container { get; set; }
        public string AncestorsId { get; set; }
        public Folder Ancestors { get; set; }
        public string ParentId { get; set; }
        public Folder Parent { get; set; }
        public string CampusId { get; set; }
        public Campus Campus { get; set; }
    }
}