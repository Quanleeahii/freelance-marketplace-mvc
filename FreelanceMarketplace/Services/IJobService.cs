using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services
{
    public interface IJobService
    {
        // Dành cho API Controller mới dùng ViewModel
        Task<Job> CreateJobAsync(JobCreateViewModel model, List<int>? skillIds = null);

        // Dành cho JobController cũ truyền trực tiếp đối tượng Job
        Task<Job> CreateJobAsync(Job job, List<int>? skillIds = null);

    }
    }
   
