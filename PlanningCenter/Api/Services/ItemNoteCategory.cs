using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class ItemNoteCategory : EntityBase
    {
        public string CreatedAt { get; set; }
        public string DeletedAt { get; set; }
        public string Name { get; set; }
        public string Sequence { get; set; }
        public string UpdatedAt { get; set; }
        public string FrequentlyUsed { get; set; }
        public string ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}