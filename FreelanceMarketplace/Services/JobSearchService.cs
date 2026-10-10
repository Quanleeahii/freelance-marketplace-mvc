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

    public async Task<(IEnumerable<Job> Items, int TotalItems)> GetJobsAsync(JobSearchViewModel model)
    {
        var query = _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Proposals)
            .Include(j => j.Skills)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.Keyword))
        {
            string keyword = model.Keyword.Trim().ToLower();
            query = query.Where(j => j.Title.ToLower().Contains(keyword) || j.Description.ToLower().Contains(keyword));
        }

        if (model.CategoryId.HasValue && model.CategoryId.Value > 0)
        {
            query = query.Where(j => j.CategoryId == model.CategoryId.Value);
        }

        if (model.SkillIds != null && model.SkillIds.Any())
        {
            query = query.Where(j => j.Skills.Any(s => model.SkillIds.Contains(s.Id)));
        }

        if (!string.IsNullOrWhiteSpace(model.Status) && model.Status != "Tất cả")
        {
            query = query.Where(j => j.Status == model.Status);
        }

        if (!string.IsNullOrWhiteSpace(model.WorkType) && model.WorkType != "Tất cả")
        {
            query = query.Where(j => j.WorkType == model.WorkType);
        }

        if (!string.IsNullOrWhiteSpace(model.PaymentType) && model.PaymentType != "Tất cả")
        {
            query = query.Where(j => j.PaymentType == model.PaymentType);
        }

        if (!string.IsNullOrWhiteSpace(model.LocationCity) && model.LocationCity != "Toàn quốc")
        {
            query = query.Where(j => j.LocationCity == model.LocationCity);
        }

        query = query.OrderByDescending(j => j.CreatedAt);

        int totalItems = await query.CountAsync();

        int pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
        int pageSize = model.PageSize > 0 ? model.PageSize : 10;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<Job?> GetJobByIdAsync(int id)
    {
        return await _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Proposals)
            .Include(j => j.Skills)
            .FirstOrDefaultAsync(j => j.Id == id);
    }
}