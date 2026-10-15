using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FreelanceMarketplace.ViewModels
{
    public class UpdateBasicInfoRequest
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
        public string? Phone { get; set; }

        [Display(Name = "Tỉnh / Thành phố")]
        [StringLength(100, ErrorMessage = "Tên tỉnh/thành phố không được vượt quá 100 ký tự")]
        public string? City { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public IFormFile? AvatarFile { get; set; }
    }
}