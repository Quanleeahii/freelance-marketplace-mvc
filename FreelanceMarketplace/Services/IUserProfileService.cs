using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services
{
    public interface IUserProfileService
    {
        Task<ManageProfileViewModel> GetManageProfileAsync(int userId);
        Task<bool> UpdateBasicInfoAsync(int userId, UpdateBasicInfoRequest request);
        Task<string> UpdateAvatarAsync(int userId, IFormFile file);
        Task<bool> UpdateFreelancerProfileAsync(int userId, UpdateFreelancerProfileRequest request);
        Task<bool> UpdateClientProfileAsync(int userId, UpdateClientProfileRequest request);
    }
}