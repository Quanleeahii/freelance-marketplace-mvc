using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    public JobService(AppDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
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
                DisplayName = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClientProfiles.Add(clientProfile);
            await _context.SaveChangesAsync(); // Lưu để sinh ra clientProfile.Id
        }
        string? attachmentUrl = null;
        if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
        {
            var allowedExts = new[] { ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg", ".zip", ".rar" };
            attachmentUrl = await _fileStorageService.SaveFileAsync(model.AttachmentFile, "job-attachments", allowedExts);
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
            Deadline = model.Deadline.HasValue
    ? DateTime.SpecifyKind(model.Deadline.Value, DateTimeKind.Utc)
    : null,
            AttachmentUrl = attachmentUrl,
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
            .Include(c => c.SubCategories)
            .OrderBy(c => c.DisplayOrder)
            .AsNoTracking()
            .ToListAsync();
    }
}