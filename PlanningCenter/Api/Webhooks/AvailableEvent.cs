using JsonApi;

namespace PlanningCenter.Api.Webhooks
{
    public class AvailableEvent : EntityBase
    {
        public string Name { get; set; }
        public string App { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }
    }
}