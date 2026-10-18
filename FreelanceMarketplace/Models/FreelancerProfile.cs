using FreelanceMarketplace.Enums;

namespace FreelanceMarketplace.Models
{
    public class FreelancerProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? Bio { get; set; }
        public string? PersonalWebsite { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; } 
        public WorkType? WorkType { get; set; }
        public bool IsAvailable { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } = null!;
        public List<Proposal> Proposals { get; set; } = new();
    }
}
