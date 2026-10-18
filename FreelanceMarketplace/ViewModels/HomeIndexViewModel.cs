using System.Collections.Generic;
using FreelanceMarketplace.Models; // Giả sử chứa Job, FreelancerProfile

namespace FreelanceMarketplace.ViewModels
{
    public class HomeIndexViewModel
    {
        // 1. Danh sách công việc mới nhất / đang mở
        public List<Job> FeaturedJobs { get; set; } = new List<Job>();

        // 2. Danh sách Freelancer nổi bật (có điểm rating cao)
        public List<UserProfile> TopFreelancers { get; set; } = new List<UserProfile>();

        // 3. Số liệu thống kê sàn (Tùy chọn)
        public int TotalJobs { get; set; }
        public int TotalFreelancers { get; set; }
    }
}