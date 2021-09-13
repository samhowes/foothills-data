using JsonApiSerializer.JsonApi;

namespace PlanningCenter.Api
{
    public class EntityBase
    {
        public string Id { get; set; }
        public Links Links { get; set; }
    }
}