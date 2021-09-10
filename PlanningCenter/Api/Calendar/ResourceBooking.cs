namespace PlanningCenter.Api.Calendar
{
    public class ResourceBooking : EntityBase
    {
        public string CreatedAt { get; set; }
        public string EndsAt { get; set; }
        public string StartsAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Quantity { get; set; }
    }
}