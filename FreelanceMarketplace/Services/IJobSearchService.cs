using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services;

public interface IJobSearchService
{
    Task<PagedResult<JobListItemDto>> GetJobsAsync(JobFilterRequest request);
    Task<Job?> GetJobByIdAsync(int id);
}