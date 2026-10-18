namespace FreelanceMarketplace.ViewModels
{
    public class ManageProfileViewModel
    {
        public UserProfileViewModel BasicInfo { get; set; } = new();
        public FreelancerProfileViewModel? FreelancerInfo { get; set; }
        public ClientProfileViewModel? ClientInfo { get; set; }
    }
}