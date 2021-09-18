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
    }
}