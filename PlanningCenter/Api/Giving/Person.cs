namespace PlanningCenter.Api.Giving
{
    public class Person : EntityBase
    {
        public string Permissions { get; set; }
        public string EmailAddresses { get; set; }
        public string Addresses { get; set; }
        public string PhoneNumbers { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DonorNumber { get; set; }
    }
}