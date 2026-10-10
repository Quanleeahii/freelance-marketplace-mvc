using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels
{
    public class JobCreateViewModel
    {
        [Required(ErrorMessage = "ClientProfileId không được để trống.")]
        public int ClientProfileId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên mục (CategoryId).")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tiêu đề dự án không được để trống.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả dự án không được để trống.")]
        public string Description { get; set; } = string.Empty;

        public string? WorkType { get; set; }
        public string? PaymentType { get; set; }
        public string? LocationCity { get; set; }
        public decimal? BudgetMin { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
