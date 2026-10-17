using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class JobSearchService : IJobSearchService
{
    private readonly AppDbContext _context;

    public JobSearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Job>> GetJobsAsync(JobFilterRequest request)
    {
        var query = _context.Jobs
            .AsNoTracking()
            .Include(j => j.Category)
            .Include(j => j.Proposals)
            .Include(j => j.Skills)
            .Include(j => j.ClientProfile)
            .ThenInclude(cp => cp.User)
            .Where(j => j.Status == "Open")
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            string keyword = request.Keyword.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(keyword) ||                           
                                     j.Skills.Any(s => s.Name.ToLower().Contains(keyword)));
        }
        if (!string.IsNullOrWhiteSpace(request.LocationCity))
        {
            string city = request.LocationCity.Trim().ToLower();
            query = query.Where(j => j.LocationCity != null && j.LocationCity.ToLower().Contains(city));
        }
        if (request.SubCategoryId.HasValue && request.SubCategoryId.Value > 0)
        {
            query = query.Where(j => j.CategoryId == request.SubCategoryId.Value);
        }
        else if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
        {
            query = query.Where(j => j.CategoryId == request.CategoryId.Value ||
                                     j.Category.ParentId == request.CategoryId.Value);
        }
        if (request.SkillIds != null && request.SkillIds.Any())
        {
            query = query.Where(j => j.Skills.Any(s => request.SkillIds.Contains(s.Id)));
        }

        if (request.WorkType.HasValue)
        {
            query = query.Where(j => j.WorkType == request.WorkType.Value);
        }

        if (request.WorkMode.HasValue)
        {
            query = query.Where(j => j.WorkMode == request.WorkMode.Value);
        }

        if (request.PaymentType.HasValue)
        {
            query = query.Where(j => j.PaymentType == request.PaymentType.Value);
        }
        query = query.OrderByDescending(j => j.CreatedAt);
        int totalCount = await query.CountAsync();
        int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        int pageSize = request.PageSize > 0 ? request.PageSize : 10;
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var items = await query
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
            .AsNoTracking()
            .Include(j => j.Category)
            .Include(j => j.Proposals)
            .Include(j => j.Skills)
            .FirstOrDefaultAsync(j => j.Id == id);
    }
}