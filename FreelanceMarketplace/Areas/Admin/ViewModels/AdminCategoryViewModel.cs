using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Areas.Admin.ViewModels
{
    public class AdminCategoryViewModel
    {
        public List<Category> ParentCategoriesWithSubs { get; set; } = new();
        public List<Category> ParentListForDropdown { get; set; } = new();
    }
}