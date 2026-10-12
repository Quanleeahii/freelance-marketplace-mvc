using FreelanceMarketplace.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace FreelanceMarketplace.ViewModels;

public class JobCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn lĩnh vực cần tuyển")]
    [Display(Name = "Lĩnh vực cần tuyển")]
    public int? ParentCategoryId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn dịch vụ phù hợp")]
    [Display(Name = "Dịch vụ phù hợp")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề công việc")]
    [MinLength(10, ErrorMessage = "Tiêu đề công việc quá ngắn")]
    [MaxLength(200, ErrorMessage = "Tiêu đề công việc quá dài")]
    [Display(Name = "Tiêu đề công việc")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mô tả chi tiết công việc")]
    [MinLength(20, ErrorMessage = "Mô tả công việc quá ngắn")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Mô tả chi tiết")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn loại hình làm việc")]
    [Display(Name = "Loại hình làm việc")]
    public WorkType WorkType { get; set; } = WorkType.PartTime;

    [Required(ErrorMessage = "Vui lòng chọn hình thức làm việc")]
    [Display(Name = "Hình thức làm việc")]
    public WorkMode WorkMode { get; set; } = WorkMode.Online;

    [Required(ErrorMessage = "Vui lòng chọn hình thức trả lương")]
    [Display(Name = "Hình thức trả lương")]
    public PaymentType PaymentType { get; set; } = PaymentType.ByProject;

    [Display(Name = "Nơi cần tuyển freelancer")]
    [StringLength(100, ErrorMessage = "Địa điểm tối đa 100 ký tự")]
    public string? LocationCity { get; set; }

    [Display(Name = "Ngân sách tối thiểu (VNĐ)")]
    [Range(200000, (double)decimal.MaxValue, ErrorMessage = "Ngân sách tối thiểu từ 200.000 VNĐ trở lên")]
    [DataType(DataType.Currency)]
    public decimal? BudgetMin { get; set; }

    [Display(Name = "Ngân sách tối đa (VNĐ)")]
    [Range(200000, (double)decimal.MaxValue, ErrorMessage = "Ngân sách tối đa từ 200.000 VNĐ trở lên")]
    [DataType(DataType.Currency)]
    [Remote(action: "VerifyBudget", controller: "Job", AdditionalFields = nameof(BudgetMin), ErrorMessage = "Ngân sách tối đa phải lớn hơn ngân sách tối thiểu")]
    public decimal? BudgetMax { get; set; }

    [Display(Name = "Hạn chót nhận hồ sơ")]
    [DataType(DataType.DateTime)]
    public DateTime? Deadline { get; set; }

    [Display(Name = "Kỹ năng yêu cầu")]
    public List<int> SkillIds { get; set; } = new();
}