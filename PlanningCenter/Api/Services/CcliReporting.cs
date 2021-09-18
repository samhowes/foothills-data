using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class CcliReporting : EntityBase
    {
        public string Digital { get; set; }
        public string Print { get; set; }
        public string Recording { get; set; }
        public string Translation { get; set; }
        public string ItemId { get; set; }
        public Item Item { get; set; }
    }
}