using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public class AdminSkillService : IAdminSkillService
    {
        private readonly AppDbContext _context;

        public AdminSkillService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> CreateSkillAsync(string name, string? slug)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Tên kỹ năng không được để trống!");
            }

            var trimmedName = name.Trim();

            // Kiểm tra trùng tên kỹ năng
            bool isExist = await _context.Skills.AnyAsync(s => s.Name.ToLower() == trimmedName.ToLower());
            if (isExist)
            {
                return (false, $"Kỹ năng '{trimmedName}' đã tồn tại trên sàn!");
            }

            // Tự động sinh slug nếu không nhập
            var finalSlug = string.IsNullOrWhiteSpace(slug)
                ? Regex.Replace(trimmedName.ToLower(), @"[^a-z0-9\s-]", "")
                : slug.Trim().ToLower();

            finalSlug = Regex.Replace(finalSlug, @"\s+", "-");

            var skill = new Skill
            {
                Name = trimmedName,
                Slug = finalSlug
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return (true, $"Thêm kỹ năng '{skill.Name}' thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteSkillAsync(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return (false, "Không tìm thấy kỹ năng yêu cầu!");
            }

            // Kiểm tra xem kỹ năng có đang được liên kết với bài đăng tuyển dụng nào không
            bool isUsedInJobs = await _context.Jobs.AnyAsync(j => j.Skills.Any(s => s.Id == id));
            if (isUsedInJobs)
            {
                return (false, $"Không thể xóa kỹ năng '{skill.Name}' vì đã có bài đăng tuyển dụng đang sử dụng!");
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return (true, $"Đã xóa kỹ năng '{skill.Name}' thành công!");
        }
    }
}