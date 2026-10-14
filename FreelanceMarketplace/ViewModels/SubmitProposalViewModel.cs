using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels;

public class SubmitProposalViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Dự án không hợp lệ")]
    public int JobId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mức chi phí bạn đề xuất")]
    [Range(100000, 1000000000, ErrorMessage = "Chi phí đề xuất tối thiểu 100.000 VNĐ")]
    [Display(Name = "Chi phí đề xuất (VNĐ)")]
    public decimal BidAmount { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thời gian hoàn thành")]
    [Range(1, 365, ErrorMessage = "Thời gian hoàn thành từ 1 đến 365 ngày")]
    [Display(Name = "Dự kiến hoàn thành (ngày)")]
    public int DeliveryDays { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung thuyết phục khách hàng.")]
    [MinLength(10, ErrorMessage = "Nội dung tối thiểu 10 ký tự")]
    [Display(Name = "Đề xuất thuyết phục")]
    public string CoverLetter { get; set; } = string.Empty;

    [Url(ErrorMessage = "Đường dẫn đính kèm phải là URL hợp lệ (ví dụ: https://...).")]
    [Display(Name = "Tài liệu đính kèm / Link sản phẩm mẫu")]
    public string? AttachmentUrl { get; set; }

    [Required(ErrorMessage = "Vui lòng cung cấp số điện thoại liên hệ")]
    [RegularExpression(@"^0[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Số điện thoại Việt Nam không hợp lệ (gồm 10 số, bắt đầu bằng 03, 05, 07, 08, 09)")]
    [Display(Name = "Số điện thoại liên hệ")]
    public string PhoneNumber { get; set; } = string.Empty;
}
