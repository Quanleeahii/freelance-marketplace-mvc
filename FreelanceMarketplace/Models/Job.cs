using FreelanceMarketplace.Enums;
namespace FreelanceMarketplace.Models
{
    public class Job
    {
        public int Id { get; set; }
        public int ClientProfileId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public WorkType WorkType { get; set; } = WorkType.PartTime;
        public WorkMode WorkMode { get; set; } = WorkMode.Online;
        public PaymentType PaymentType { get; set; } = PaymentType.ByProject;
        public string? LocationCity { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? AttachmentUrl { get; set; }
        public Category Category { get; set; } = null!;
        public ClientProfile ClientProfile { get; set; } = null!;
        public List<Proposal> Proposals { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
    }
}
