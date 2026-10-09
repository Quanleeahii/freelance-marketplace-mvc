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
        public string WorkType { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string? LocationCity { get; set; }
        public decimal BudgetMin { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Category Category { get; set; } = null!;
        public List<Proposal> Proposals { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
    }
}
