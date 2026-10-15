using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _context;

    public JobService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Job> CreateJobAsync(JobCreateViewModel model, int userId, List<int>? skillIds = null)
    {
        var clientProfile = await _context.ClientProfiles
        .FirstOrDefaultAsync(cp => cp.UserId == userId);

        // 2. Nếu chưa có (người dùng mới đăng bài lần đầu) -> Tự tạo ngầm
        if (clientProfile == null)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            clientProfile = new ClientProfile
            {
                UserId = userId,
                DisplayName = userProfile?.FullName,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClientProfiles.Add(clientProfile);
            await _context.SaveChangesAsync(); // Lưu để sinh ra clientProfile.Id
        }
        var job = new Job
        {
            ClientProfileId = clientProfile.Id,
            CategoryId = model.CategoryId,
            Title = model.Title,
            Description = model.Description,
            WorkType = model.WorkType,
            WorkMode = model.WorkMode,
            PaymentType = model.PaymentType,
            LocationCity = model.LocationCity ?? string.Empty,
            BudgetMin = model.BudgetMin ?? 0m,
            BudgetMax = model.BudgetMax,
            Deadline = model.Deadline,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        var selectedSkillIds = skillIds ?? model.SkillIds;
        if (selectedSkillIds != null && selectedSkillIds.Any())
        {
            var skills = await _context.Skills
                .Where(s => selectedSkillIds.Contains(s.Id))
                .ToListAsync();

            job.Skills = skills;
        }

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return job;
    }

    public async Task<Job> CreateJobAsync(Job job, List<int>? skillIds = null)
    {
        if (job.CreatedAt == default)
        {
            job.CreatedAt = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(job.Status))
        {
            job.Status = "Open";
        }

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
    public async Task<IEnumerable<Category>> GetSubCategoriesByParentIdAsync(int parentId)
    {
        return await _context.Categories
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
    {
        return await _context.Skills
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }
}