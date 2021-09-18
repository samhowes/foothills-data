using JsonApi;

namespace PlanningCenter.Api.Services
{
    public class BlockoutDate : EntityBase
    {
        public string GroupIdentifier { get; set; }
        public string Reason { get; set; }
        public string TimeZone { get; set; }
        public string Share { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string EndsAtUtc { get; set; }
        public string StartsAtUtc { get; set; }
        public string PersonId { get; set; }
        public Person Person { get; set; }
        public string BlockoutId { get; set; }
        public Blockout Blockout { get; set; }
    }
}