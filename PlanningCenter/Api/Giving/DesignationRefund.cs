using JsonApi;

namespace PlanningCenter.Api.Giving
{
    public class DesignationRefund : EntityBase
    {
        public string AmountCents { get; set; }
        public string AmountCurrency { get; set; }
        public string DesignationId { get; set; }
        public Designation Designation { get; set; }
    }
}