using FreelanceMarketplace.Enums;

namespace FreelanceMarketplace.ViewModels;

public class JobListItemDto
{
    public int Id { get; set; }
    public int ClientProfileId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public WorkType WorkType { get; set; }
    public WorkMode WorkMode { get; set; }
    public PaymentType PaymentType { get; set; }
    public string? LocationCity { get; set; } = string.Empty;
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AttachmentUrl { get; set; }

    public CategoryBriefDto? Category { get; set; }
    public List<SkillBriefDto> Skills { get; set; } = new();
    public ClientBriefDto ClientProfile { get; set; } = null!;
}

public class CategoryBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int? ParentId { get; set; }
}

public class SkillBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ClientBriefDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}