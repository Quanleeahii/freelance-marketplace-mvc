using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

public interface IJobService
{
    Task<Job> CreateJobAsync(JobCreateViewModel model, int userId, List<int>? skillIds = null);
    Task<IEnumerable<Category>> GetParentCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesByParentIdAsync(int parentId);
    Task<IEnumerable<Skill>> GetAllSkillsAsync();
}