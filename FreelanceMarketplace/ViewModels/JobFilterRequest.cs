using FreelanceMarketplace.Enums;

namespace FreelanceMarketplace.ViewModels;

public class JobFilterRequest
{
    public string? Keyword { get; set; }
    public string? LocationCity { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public List<int> SkillIds { get; set; } = new();
    public WorkType? WorkType { get; set; }
    public PaymentType? PaymentType { get; set; }
    public WorkMode? WorkMode { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}