using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services;

public interface IJobSearchService
{
    Task<PagedResult<Job>> GetJobsAsync(JobFilterRequest request);
    Task<Job?> GetJobByIdAsync(int id);
}