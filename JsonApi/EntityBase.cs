using JsonApiSerializer.JsonApi;

namespace JsonApi
{
    public class EntityBase
    {
        public string? Id { get; set; }
        public Links? Links { get; set; }
    }
}