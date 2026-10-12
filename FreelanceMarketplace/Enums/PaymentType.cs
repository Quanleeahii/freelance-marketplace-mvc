using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.Enums;

public enum PaymentType
{
    [Display(Name = "Trả theo dự án")]
    ByProject = 1,

    [Display(Name = "Trả theo giờ")]
    ByHour = 2,

    [Display(Name = "Trả theo tháng")]
    ByMonth = 3
}