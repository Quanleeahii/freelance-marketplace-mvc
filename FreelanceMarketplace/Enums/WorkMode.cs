using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.Enums;

public enum WorkMode
{
    [Display(Name = "Làm online")]
    Online = 1,

    [Display(Name = "Làm tại văn phòng")]
    Onsite = 2
}