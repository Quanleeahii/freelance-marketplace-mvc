namespace FreelanceMarketplace.Models
{
    public class ClientProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Description { get; set; }
        public string? DisplayName { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = null!;
        public List<Job> Jobs { get; set; } = new();
    }
}
