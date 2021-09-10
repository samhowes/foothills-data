namespace PlanningCenter.Api.CheckIns
{
    public class CheckInTime
    {
        public string Kind { get; set; }
        public string HasValidated { get; set; }
        public string Errors { get; set; }
        public string ServicesIntegrated { get; set; }
    }
}