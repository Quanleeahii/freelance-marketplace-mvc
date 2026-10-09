using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services
{
    public interface IAuthService
    {
        Task<bool> IsEmailTakenAsync(string email);
        Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterViewModel model);

    }
}
