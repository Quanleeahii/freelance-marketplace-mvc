using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels;

public class SubmitProposalViewModel
{
    [Required(ErrorMessage = "Mã công việc không được để trống.")]
    public int JobId { get; set; }

    public int FreelancerUserId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá chào thầu.")]
    [Range(1000, 1000000000, ErrorMessage = "Giá chào thầu phải từ 1.000 VNĐ trở lên.")]
    [Display(Name = "Giá chào thầu (VNĐ)")]
    public decimal BidAmount { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số ngày dự kiến hoàn thành.")]
    [Range(1, 365, ErrorMessage = "Số ngày hoàn thành phải từ 1 đến 365 ngày.")]
    [Display(Name = "Số ngày hoàn thành")]
    public int DeliveryDays { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thư giới thiệu.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Thư giới thiệu từ 10 đến 2000 ký tự.")]
    [Display(Name = "Thư giới thiệu")]
    public string CoverLetter { get; set; } = string.Empty;

    [Url(ErrorMessage = "Đường dẫn đính kèm phải là URL hợp lệ.")]
    [Display(Name = "Link tài liệu / Portfolio")]
    public string? AttachmentUrl { get; set; }
}

public class FreelancerProposalItemViewModel
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public int DeliveryDays { get; set; }
    public string CoverLetter { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}