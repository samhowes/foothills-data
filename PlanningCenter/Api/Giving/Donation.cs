using System.Collections.Generic;
using JsonApi;
using JsonApiSerializer.JsonApi;

namespace PlanningCenter.Api.Giving
{
    public class Donation : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PaymentMethodSub { get; set; }
        public string PaymentLast4 { get; set; }
        public string PaymentBrand { get; set; }
        public string PaymentCheckNumber { get; set; }
        public string PaymentCheckDatedAt { get; set; }
        public string FeeCents { get; set; }
        public string PaymentMethod { get; set; }
        public string ReceivedAt { get; set; }
        public string AmountCents { get; set; }
        public string PaymentStatus { get; set; }
        public string CompletedAt { get; set; }
        public string AmountCurrency { get; set; }
        public string FeeCurrency { get; set; }
        public bool Refunded { get; set; }
        public string Refundable { get; set; }
        public Batch Batch { get; set; }
        public Campus Campus { get; set; }
        public Person? Person { get; set; }
        public PaymentSource PaymentSource { get; set; }
        public RecurringDonation RecurringDonation { get; set; }
        
        public Relationship<List<Designation>> Designations { get; set; } 
    }
}