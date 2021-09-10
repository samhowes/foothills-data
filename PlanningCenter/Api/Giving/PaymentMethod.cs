namespace PlanningCenter.Api.Giving
{
    public class PaymentMethod
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string MethodType { get; set; }
        public string MethodSubtype { get; set; }
        public string Last4 { get; set; }
        public string Brand { get; set; }
        public string Expiration { get; set; }
        public string Verified { get; set; }
    }
}