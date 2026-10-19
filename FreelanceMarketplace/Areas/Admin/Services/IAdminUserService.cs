using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public interface IAdminUserService
    {
        Task<PagedResult<User>> GetUsersAsync(bool? isVerified, string? search, int pageNumber = 1, int pageSize = 10);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id);
    }
}