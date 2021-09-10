namespace PlanningCenter.Api.Services
{
    public class BlockoutDate
    {
        public string GroupIdentifier { get; set; }
        public string Reason { get; set; }
        public string TimeZone { get; set; }
        public string Share { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string EndsAtUtc { get; set; }
        public string StartsAtUtc { get; set; }
    }
}