using System.ComponentModel.DataAnnotations;
using FreelanceMarketplace.Enums;

namespace FreelanceMarketplace.ViewModels;

public class UpdateFreelancerProfileRequest
{
    [Display(Name = "Lĩnh vực chuyên môn")]
    public int? CategoryId { get; set; }

    [Display(Name = "Giới thiệu bản thân")]
    public string? Bio { get; set; }

    [Display(Name = "Website / Portfolio cá nhân")]
    [Url(ErrorMessage = "Hãy điền một địa chỉ URL đúng")]
    public string? PersonalWebsite { get; set; }

    [Display(Name = "Mức độ kinh nghiệm")]
    [EnumDataType(typeof(ExperienceLevel), ErrorMessage = "Mức độ kinh nghiệm không hợp lệ")]
    public ExperienceLevel? ExperienceLevel { get; set; }

    [Display(Name = "Thời gian làm việc")]
    [EnumDataType(typeof(WorkType), ErrorMessage = "Hình thức làm việc không hợp lệ")]
    public WorkType? WorkType { get; set; }

    [Display(Name = "Trạng thái sẵn sàng nhận việc")]
    public bool IsAvailable { get; set; }
}