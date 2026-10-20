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

    public async Task<PagedResult<JobListItemDto>> GetJobsAsync(JobFilterRequest request)
    {
        var query = _context.Jobs
        .AsNoTracking()
        .Where(j => j.Status == "Open");
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
            var workTypeValue = (int)request.WorkType.Value;
            query = query.Where(j => (int)j.WorkType == workTypeValue);
        }

        if (request.WorkMode.HasValue)
        {
            var workModeValue = (int)request.WorkMode.Value;
            query = query.Where(j => (int)j.WorkMode == workModeValue);
        }

        if (request.PaymentType.HasValue)
        {
            var paymentTypeValue = (int)request.PaymentType.Value;
            query = query.Where(j => (int)j.PaymentType == paymentTypeValue);
        }
        query = query.OrderByDescending(j => j.CreatedAt);
        int totalCount = await query.CountAsync();
        int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        int pageSize = request.PageSize > 0 ? request.PageSize : 10;
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var items = await query
        .OrderByDescending(j => j.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(j => new JobListItemDto
        {
            Id = j.Id,
            ClientProfileId = j.ClientProfileId,
            CategoryId = j.CategoryId,
            Title = j.Title,
            Description = j.Description,
            Status = j.Status,
            WorkType = j.WorkType,      
            WorkMode = j.WorkMode,      
            PaymentType = j.PaymentType,
            LocationCity = j.LocationCity,
            BudgetMin = j.BudgetMin,
            BudgetMax = j.BudgetMax,
            Deadline = j.Deadline,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt,
            AttachmentUrl = j.AttachmentUrl,
            Category = j.Category != null ? new CategoryBriefDto
            {
                Id = j.Category.Id,
                Name = j.Category.Name,
                Slug = j.Category.Slug,
                DisplayOrder = j.Category.DisplayOrder,
                ParentId = j.Category.ParentId
            } : null,
            Skills = j.Skills.Select(s => new SkillBriefDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList(),
            ClientProfile = new ClientBriefDto
            {
                Id = j.ClientProfileId,
                DisplayName = !string.IsNullOrWhiteSpace(j.ClientProfile.DisplayName)
        ? j.ClientProfile.DisplayName
        : (j.ClientProfile.User.UserProfile != null && !string.IsNullOrWhiteSpace(j.ClientProfile.User.UserProfile.FullName)
            ? j.ClientProfile.User.UserProfile.FullName
            : "Khách hàng")
            }
        })
        .ToListAsync();
        return new PagedResult<JobListItemDto>
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
            .Include(j => j.Skills)
            .Include(j => j.ClientProfile)
            .ThenInclude(cp => cp.User)
                .ThenInclude(u => u.UserProfile)
            .Include(j => j.Proposals)
                .ThenInclude(p => p.FreelancerProfile)
                    .ThenInclude(fp => fp.User)
                        .ThenInclude(u => u.UserProfile)
            .FirstOrDefaultAsync(j => j.Id == id);
    }
}