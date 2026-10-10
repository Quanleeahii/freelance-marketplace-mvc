using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services
{
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Job> CreateJobAsync(Job job, List<int>? skillIds = null)
        {
            if (job.CreatedAt == default) job.CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(job.Status)) job.Status = "Open";

            job.Category = null;

            if (skillIds != null && skillIds.Any())
            {
                var skills = await _context.Skills
                    .Where(s => skillIds.Contains(s.Id))
                    .ToListAsync();
                job.Skills = skills;
            }

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<(List<Job> Items, int TotalItems)> GetJobsAsync(
            string? search,
            int? categoryId,
            string? workType,
            string? paymentType,
            string? locationCity,
            string? status,
            int pageNumber = 1,
            int pageSize = 6)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Skills)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(j => j.Status.ToLower() == status.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();
                query = query.Where(j => j.Title.ToLower().Contains(keyword)
                                      || j.Description.ToLower().Contains(keyword));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(j => j.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(workType))
                query = query.Where(j => j.WorkType.ToLower() == workType.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(paymentType))
                query = query.Where(j => j.PaymentType.ToLower() == paymentType.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(locationCity))
                query = query.Where(j => j.LocationCity != null && j.LocationCity.ToLower() == locationCity.Trim().ToLower());

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }
        var job = new Job
        {
            ClientProfileId = model.ClientProfileId > 0 ? model.ClientProfileId : 1,
            CategoryId = model.CategoryId,
            Title = model.Title,
            Description = model.Description,
            WorkType = model.WorkType ?? string.Empty,
            PaymentType = model.PaymentType ?? string.Empty,
            LocationCity = model.LocationCity ?? string.Empty,
            BudgetMin = model.BudgetMin ?? 0m,
            Deadline = model.Deadline,
            Category = null!
        };

        _context.Jobs.Add(job);
    await _context.SaveChangesAsync();

    return job;

        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Skills)
                .FirstOrDefaultAsync(j => j.Id == id);
        }
    }
}
