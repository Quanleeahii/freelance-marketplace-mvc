namespace FreelanceMarketplace.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}