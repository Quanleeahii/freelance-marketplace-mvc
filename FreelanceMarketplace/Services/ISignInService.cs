using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Services
{
    public interface ISignInService
    {
        Task SignInUserAsync(User user, bool isPersistent = false);
        Task SignOutUserAsync();
    }
}
