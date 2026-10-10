using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels
{
    public class JobCreateViewModel
    {
        public int ClientProfileId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên mục.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        public string Description { get; set; } = string.Empty;

        public string? WorkType { get; set; }
        public string? PaymentType { get; set; }
        public string? LocationCity { get; set; }
        public decimal? BudgetMin { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
