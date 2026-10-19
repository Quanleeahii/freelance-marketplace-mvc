using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly AppDbContext _context;

        public AdminCategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesWithHierarchyAsync()
        {
            // Lấy danh sách danh mục cha cùng toàn bộ chuyên ngành con của từng cha
            return await _context.Categories
                .Where(c => c.ParentId == null)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Category>> GetParentCategoriesAsync()
        {
            // Chỉ lấy danh mục cấp 1 để nạp vào thẻ dropdown chọn cha
            return await _context.Categories
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> CreateCategoryAsync(string name, string? slug, int? parentId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Tên lĩnh vực không được để trống!");
            }

            var trimmedName = name.Trim();

            // Kiểm tra trùng tên trong cùng cấp bậc
            bool isExist = await _context.Categories
                .AnyAsync(c => c.ParentId == parentId && c.Name.ToLower() == trimmedName.ToLower());

            if (isExist)
            {
                return (false, $"Tên '{trimmedName}' đã tồn tại trong nhóm này!");
            }

            // Tự sinh slug nếu để trống
            var finalSlug = string.IsNullOrWhiteSpace(slug)
                ? Regex.Replace(trimmedName.ToLower(), @"[^a-z0-9\s-]", "")
                : slug.Trim().ToLower();

            finalSlug = Regex.Replace(finalSlug, @"\s+", "-");

            var category = new Category
            {
                Name = trimmedName,
                Slug = finalSlug,
                ParentId = parentId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return (true, $"Thêm thành công: '{category.Name}'!");
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return (false, "Không tìm thấy danh mục yêu cầu!");
            }

            // 1. Chặn xóa nếu danh mục cha đang có các chuyên ngành con bên trong
            if (category.SubCategories != null && category.SubCategories.Any())
            {
                return (false, $"Không thể xóa '{category.Name}' vì danh mục này đang chứa {category.SubCategories.Count} chuyên ngành con!");
            }

            // 2. Chặn xóa nếu đã có bài đăng tuyển dụng liên kết với danh mục này
            bool isUsedInJobs = await _context.Jobs.AnyAsync(j => j.CategoryId == id);
            if (isUsedInJobs)
            {
                return (false, $"Không thể xóa '{category.Name}' vì đã có bài đăng việc làm chọn danh mục này!");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return (true, $"Đã xóa '{category.Name}' thành công!");
        }
    }
}