namespace PlanningCenter.Api.Giving
{
    public class PledgeCampaign : EntityBase
    {
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string GoalCents { get; set; }
        public string GoalCurrency { get; set; }
        public string ShowGoalInChurchCenter { get; set; }
        public string ReceivedTotalFromPledgesCents { get; set; }
        public string ReceivedTotalOutsideOfPledgesCents { get; set; }
    }
}