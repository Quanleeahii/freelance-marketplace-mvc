using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.Enums;

public enum ExperienceLevel
{
    [Display(Name = "Sinh viên")]
    Student = 1,

    [Display(Name = "Mới đi làm")]
    Entry = 2,

    [Display(Name = "Đã có kinh nghiệm")]
    Intermediate = 3,

    [Display(Name = "Chuyên gia")]
    Expert = 4
}