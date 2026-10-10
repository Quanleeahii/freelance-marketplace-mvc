using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels;

public class JobSearchViewModel
{
    [Display(Name = "Keyword")]
    [StringLength(100, ErrorMessage = "Từ khóa tìm kiếm không được vượt quá 100 ký tự.")]
    public string? Keyword { get; set; }

    [Display(Name = "Category ID")]
    public int? CategoryId { get; set; }

    [Display(Name = "Service ID")]
    public int? ServiceId { get; set; }

    [Display(Name = "Skill IDs")]
    public List<int>? SkillIds { get; set; }

    [Display(Name = "Status")]
    public string? Status { get; set; }

    [Display(Name = "Work Type")]
    public string? WorkType { get; set; }

    [Display(Name = "Payment Type")]
    public string? PaymentType { get; set; }

    [Display(Name = "Location City")]
    public string? LocationCity { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn hoặc bằng 1.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Số lượng bản ghi mỗi trang phải từ 1 đến 100.")]
    public int PageSize { get; set; } = 10;
}
