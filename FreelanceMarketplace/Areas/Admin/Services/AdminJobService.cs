using FreelanceMarketplace.Areas.Admin.Services;
using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Areas.Admin.Services
{
    public class AdminJobService : IAdminJobService
    {
        private readonly AppDbContext _context;

        public AdminJobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Job>> GetJobsAsync(string? status, string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.ClientProfile)
                    .ThenInclude(cp => cp.User)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(j => j.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                query = query.Where(j =>
                    j.Title.ToLower().Contains(lowerKeyword) ||
                    (j.ClientProfile != null && j.ClientProfile.User != null && j.ClientProfile.User.Email.ToLower().Contains(lowerKeyword)));
            }

            int totalCount = await query.CountAsync();
            pageNumber = pageNumber > 0 ? pageNumber : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Job>
            {
                Items = items,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Skills)
                .Include(j => j.ClientProfile)
                    .ThenInclude(cp => cp.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<bool> ToggleJobStatusAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return false;

            job.Status = (job.Status == "Hidden") ? "Open" : "Hidden";
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}