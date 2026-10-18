using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels;

public class UpdateClientProfileRequest
{
    [Display(Name = "Tên hiển thị / Tên công ty")]
    [StringLength(150, ErrorMessage = "Tên hiển thị tối đa 150 ký tự")]
    public string? DisplayName { get; set; }

    [Display(Name = "Website công ty")]
    [StringLength(255, ErrorMessage = "Đường dẫn website tối đa 255 ký tự")]
    [Url(ErrorMessage = "Đường dẫn website không hợp lệ (cần có http:// hoặc https://)")]
    public string? Website { get; set; }

    [Display(Name = "Giới thiệu doanh nghiệp")]
    [StringLength(2000, ErrorMessage = "Giới thiệu doanh nghiệp tối đa 2000 ký tự")]
    public string? Description { get; set; }
}