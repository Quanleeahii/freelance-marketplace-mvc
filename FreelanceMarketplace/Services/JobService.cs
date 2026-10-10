using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;
using FreelanceMarketplace.Data;

namespace FreelanceMarketplace.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _context;

    public JobService(AppDbContext context)
    {
        _context = context;
    }

    // 1. Xử lý khi truyền vào ViewModel (từ JobPostingAPIController)
    public async Task<Job> CreateJobAsync(JobCreateViewModel model, List<int>? skillIds = null)
    {
        var job = new Job
        {
            ClientProfileId = model.ClientProfileId,
            CategoryId = model.CategoryId,
            Title = model.Title,
            Description = model.Description,
            WorkType = model.WorkType ?? string.Empty,
            PaymentType = model.PaymentType ?? string.Empty,
            LocationCity = model.LocationCity ?? string.Empty,
            BudgetMin = model.BudgetMin ?? 0m,
            Deadline = model.Deadline,
            Status = "Đang mở",
            CreatedAt = DateTime.UtcNow,
            Category = null!
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return job;
    }

    // 2. Xử lý khi truyền trực tiếp đối tượng Job (từ JobController cũ)
    public async Task<Job> CreateJobAsync(Job job, List<int>? skillIds = null)
    {
        if (job.CreatedAt == default)
        {
            job.CreatedAt = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(job.Status))
        {
            job.Status = "Đang mở";
        }

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return job;
    }
}