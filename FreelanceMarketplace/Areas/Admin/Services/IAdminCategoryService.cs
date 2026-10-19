using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public interface IAdminCategoryService
    {
        Task<List<Category>> GetAllCategoriesWithHierarchyAsync();
        Task<List<Category>> GetParentCategoriesAsync();
        Task<(bool Success, string Message)> CreateCategoryAsync(string name, string? slug, int? parentId);
        Task<(bool Success, string Message)> DeleteCategoryAsync(int id);
    }
}