namespace FreelanceMarketplace.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int PendingJobsCount { get; set; }
        public int TotalJobsCount { get; set; }
        public int TotalFreelancersCount { get; set; }
        public int TotalProposalsCount { get; set; }
        public int TotalUsersCount { get; set; }
    }
}