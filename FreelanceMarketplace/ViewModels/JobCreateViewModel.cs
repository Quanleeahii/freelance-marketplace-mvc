using FreelanceMarketplace.Enums;
using System.ComponentModel.DataAnnotations;
namespace FreelanceMarketplace.ViewModels;

public class JobCreateViewModel : IValidatableObject
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

    [Display(Name = "Nơi cần tuyển Freelancer")]
    public string LocationCity { get; set; } = "Toàn quốc";
    [Display(Name = "Tài liệu đính kèm")]
    public IFormFile? AttachmentFile { get; set; }

    [Display(Name = "Ngân sách tối thiểu (VNĐ)")]
    [Range(200000, (double)decimal.MaxValue, ErrorMessage = "Ngân sách tối thiểu từ 200.000 VNĐ trở lên")]
    [DataType(DataType.Currency)]
    public decimal? BudgetMin { get; set; }

    [Display(Name = "Ngân sách tối đa (VNĐ)")]
    [Range(200000, (double)decimal.MaxValue, ErrorMessage = "Ngân sách tối đa từ 200.000 VNĐ trở lên")]
    [DataType(DataType.Currency)]
    public decimal? BudgetMax { get; set; }

    [Display(Name = "Hạn chót nhận hồ sơ")]
    [Required(ErrorMessage = "Vui lòng chọn hạn chót nhận hồ sơ")]
    [DataType(DataType.Date)]
    public DateTime? Deadline { get; set; }

    [Display(Name = "Kỹ năng yêu cầu")]
    [MinLength(1, ErrorMessage = "Vui lòng chọn ít nhất 1 kỹ năng")]
    public List<int> SkillIds { get; set; } = new();
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BudgetMin.HasValue && BudgetMax.HasValue && BudgetMax.Value <= BudgetMin.Value)
        {
            yield return new ValidationResult(
                "Ngân sách tối đa phải lớn hơn ngân sách tối thiểu",
                new[] { nameof(BudgetMax) }
            );
        }
        if (Deadline.HasValue && Deadline.Value.Date <= DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "Hạn chót nhận hồ sơ phải sau ngày hiện tại",
                new[] { nameof(Deadline) }
            );
        }
    }
}