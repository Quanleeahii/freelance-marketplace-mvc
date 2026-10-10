using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Services
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(Job job, List<int>? skillIds = null);

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
   
