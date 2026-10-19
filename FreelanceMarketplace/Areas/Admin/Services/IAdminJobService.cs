using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public interface IAdminJobService
    {
        Task<PagedResult<Job>> GetJobsAsync(string? status, string? keyword, int pageNumber = 1, int pageSize = 10);
        Task<Job?> GetJobByIdAsync(int id);
        Task<bool> ToggleJobStatusAsync(int id);
    }
}