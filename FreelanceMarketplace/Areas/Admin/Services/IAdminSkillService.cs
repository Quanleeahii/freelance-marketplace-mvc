using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public interface IAdminSkillService
    {
        Task<List<Skill>> GetAllSkillsAsync();
        Task<(bool Success, string Message)> CreateSkillAsync(string name, string? slug);
        Task<(bool Success, string Message)> DeleteSkillAsync(int id);
    }
}