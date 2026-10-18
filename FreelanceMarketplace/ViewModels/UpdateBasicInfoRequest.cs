using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.ViewModels;

public class UpdateBasicInfoRequest
{
    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(100, ErrorMessage = "Họ và tên quá dài")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng (VD: 0912345678)")]
    public string? Phone { get; set; }

    [Display(Name = "Tỉnh / Thành phố")]
    [ProvinceValidation(ErrorMessage = "Tỉnh/thành phố không hợp lệ")]
    public string? City { get; set; }

    [Display(Name = "Địa chỉ chi tiết")]
    [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
    public string? Address { get; set; }

    [StringLength(500, ErrorMessage = "Đường dẫn ảnh đại diện không hợp lệ")]
    public string? AvatarUrl { get; set; }
}

public class ProvinceValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var city = value as string;
        if (string.IsNullOrWhiteSpace(city))
        {
            return ValidationResult.Success;
        }
        if (VietnameseProvinces.List.Contains(city))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage ?? "Tỉnh/thành phố không hợp lệ");
    }
}