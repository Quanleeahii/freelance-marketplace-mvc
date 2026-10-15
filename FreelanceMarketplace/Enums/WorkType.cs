using System.ComponentModel.DataAnnotations;

namespace FreelanceMarketplace.Enums;

public enum WorkType
{
    [Display(Name = "Việc bán thời gian")]
    PartTime = 1,

    [Display(Name = "Việc toàn thời gian")]
    FullTime = 2
}