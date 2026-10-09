namespace FreelanceMarketplace.Models
{
    public class Proposal
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int FreelancerProfileId { get; set; }
        public decimal BidAmount { get; set; }
        public int DeliveryDays { get; set; }
        public string Status { get; set; } = "Pending";
        public string CoverLetter { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Job Job { get; set; } = null!;
        public FreelancerProfile Freelancer { get; set; } = null!;
    }
}
