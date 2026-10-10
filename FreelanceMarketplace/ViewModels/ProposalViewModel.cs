using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels;

/// <summary>
/// ViewModel dùng cho form nộp báo giá (Freelancer nộp hoặc test qua API)
/// </summary>
public class SubmitProposalViewModel
{
    [Required(ErrorMessage = "Mã công việc không được để trống.")]
    [Range(1, int.MaxValue, ErrorMessage = "Mã công việc không hợp lệ.")]
    [Display(Name = "Mã dự án")]
    public int JobId { get; set; }

    [Display(Name = "Mã ứng viên")]
    public int FreelancerUserId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá chào thầu.")]
    [Range(1000, 10000000000, ErrorMessage = "Giá chào thầu phải từ {1:N0} VNĐ đến {2:N0} VNĐ.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Giá chào thầu (VNĐ)")]
    public decimal BidAmount { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thời gian hoàn thành dự kiến.")]
    [Range(1, 365, ErrorMessage = "Thời gian hoàn thành phải từ 1 đến 365 ngày.")]
    [Display(Name = "Thời gian hoàn thành (ngày)")]
    public int DeliveryDays { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thư giới thiệu.")]
    [StringLength(2000, MinimumLength = 20, ErrorMessage = "Thư giới thiệu phải có độ dài từ {2} đến {1} ký tự.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Thư giới thiệu")]
    public string CoverLetter { get; set; } = string.Empty;

    [Url(ErrorMessage = "Đường dẫn tệp đính kèm không đúng định dạng URL.")]
    [MaxLength(500, ErrorMessage = "Đường dẫn đính kèm không được vượt quá 500 ký tự.")]
    [Display(Name = "Đường dẫn tệp đính kèm")]
    public string? AttachmentUrl { get; set; }
}

/// <summary>
/// ViewModel dùng khi Client duyệt báo giá
/// </summary>
public class AcceptProposalViewModel
{
    [Required(ErrorMessage = "Mã báo giá không được để trống.")]
    [Range(1, int.MaxValue, ErrorMessage = "Mã báo giá không hợp lệ.")]
    [Display(Name = "Mã báo giá")]
    public int ProposalId { get; set; }

    [Display(Name = "Mã dự án")]
    public int JobId { get; set; }

    [Display(Name = "Mã khách hàng")]
    public int ClientUserId { get; set; }
}