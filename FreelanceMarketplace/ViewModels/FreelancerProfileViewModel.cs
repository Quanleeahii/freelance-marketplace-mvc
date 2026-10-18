using FreelanceMarketplace.Enums;

namespace FreelanceMarketplace.ViewModels;

public class FreelancerProfileViewModel
{
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Bio { get; set; }
    public string? PersonalWebsite { get; set; }
    public ExperienceLevel? ExperienceLevel { get; set; }
    public WorkType? WorkType { get; set; }
    public bool IsAvailable { get; set; }
}