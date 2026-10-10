using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(JobCreateViewModel model, List<int>? skillIds);

        Task<(List<Job> Items, int TotalItems)> GetJobsAsync(
            string? search,
            int? categoryId,
            string? workType,
            string? paymentType,
            string? locationCity,
            string? status,
            int pageNumber = 1,
            int pageSize = 6);

        Task<Job?> GetJobByIdAsync(int id);

    }
    }
   
