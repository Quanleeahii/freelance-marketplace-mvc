using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services;

public interface IJobSearchService
{
    Task<(IEnumerable<Job> Items, int TotalItems)> GetJobsAsync(JobSearchViewModel model);
    Task<Job?> GetJobByIdAsync(int id);
}
