namespace FreelanceMarketplace.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int PendingJobsCount { get; set; }
        public int TotalJobsCount { get; set; }
        public int VerifiedUsersCount { get; set; }
        public int InProgressJobsCount { get; set; }
        public int TotalUsersCount { get; set; }
    }
}