namespace FreelanceMarketplace.Models
{
    public class UserProfile
    {
        public int Id {  get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = null!;
    }
}
