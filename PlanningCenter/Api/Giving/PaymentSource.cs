using JsonApi;

namespace PlanningCenter.Api.Giving
{
    public class PaymentSource : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Name { get; set; }
    }
}