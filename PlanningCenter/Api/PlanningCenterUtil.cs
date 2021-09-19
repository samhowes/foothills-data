using PlanningCenter.Api.Giving;
using PlanningCenter.Api.Groups;

namespace PlanningCenter.Api
{
    public static class PlanningCenterUtil
    {
        public static string DonationLink(Donation donation) =>
            $"https://giving.planningcenteronline.com/donations/{donation.Id}";
        public static string GroupLink(Group group)
            => $"https://groups.planningcenteronline.com/groups/{group.Id}";

        public static string CheckInsLink(string eventPeriodId, string checkInId)
            => $"https://check-ins.planningcenteronline.com/event_periods/{eventPeriodId}/check_ins/{checkInId}";

        public static string PersonLink(string personId)
            => $"https://people.planningcenteronline.com/people/AC{personId}";
    }
}