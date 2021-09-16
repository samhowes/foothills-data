using System.Collections.Generic;
using JsonApiSerializer.JsonApi;

namespace PlanningCenter.Api.CheckIns
{
    public class CheckIn : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MedicalNotes { get; set; }
        public string Kind { get; set; }
        public string Number { get; set; }
        public string SecurityCode { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CheckedOutAt { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhoneNumber { get; set; }
        public string EventPeriodId { get; set; }
        public EventPeriod EventPeriod { get; set; }
        public Person? Person { get; set; }
        
        public Relationship<List<EventTime>> EventTimes { get; set; }
    }
}