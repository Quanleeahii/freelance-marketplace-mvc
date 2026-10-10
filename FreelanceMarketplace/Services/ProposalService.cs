using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class ProposalService : IProposalService
{
    private readonly AppDbContext _context;

    public ProposalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> SubmitProposalAsync(
        int jobId,
        int freelancerUserId,
        decimal bidAmount,
        int deliveryDays,
        string coverLetter,
        string? attachmentUrl = null)
    {

        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
            return (false, "Công việc này không tồn tại.");

        if (job.Status != "Open")
            return (false, "Công việc này đã đóng hoặc đã giao cho người khác.");


        var freelancerProfile = await _context.FreelancerProfiles
            .FirstOrDefaultAsync(f => f.UserId == freelancerUserId);

        if (freelancerProfile == null)
        {

            var user = await _context.Users.FindAsync(freelancerUserId);
            if (user == null)
            {
                user = new User
                {
                    Email = $"freelancer_{freelancerUserId}@gmail.com",
                    PasswordHash = "Password123@",
                    Role = "Freelancer",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                freelancerUserId = user.Id;
            }

            freelancerProfile = new FreelancerProfile
            {
                UserId = freelancerUserId
            };
            _context.FreelancerProfiles.Add(freelancerProfile);
            await _context.SaveChangesAsync();
        }


        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.UserId == freelancerUserId);

        if (userProfile != null && job.ClientProfileId == userProfile.Id)
            return (false, "Bạn không thể gửi báo giá cho dự án của chính mình.");


        bool alreadyBid = await _context.Proposals
            .AnyAsync(p => p.JobId == jobId && p.FreelancerProfileId == freelancerProfile.Id);

        if (alreadyBid)
            return (false, "Bạn đã gửi báo giá cho dự án này rồi.");


        var proposal = new Proposal
        {
            JobId = jobId,
            FreelancerProfileId = freelancerProfile.Id,
            BidAmount = bidAmount,
            DeliveryDays = deliveryDays,
            CoverLetter = coverLetter,
            AttachmentUrl = attachmentUrl,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();

        return (true, "Gửi báo giá thành công!");
    }

    public async Task<List<Proposal>> GetProposalsByJobIdAsync(int jobId, int clientUserId)
    {

        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.UserId == clientUserId);

        if (userProfile == null) return new List<Proposal>();

        var job = await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == jobId && j.ClientProfileId == userProfile.Id);

        if (job == null) return new List<Proposal>();

        return await _context.Proposals
            .AsNoTracking()
            .Where(p => p.JobId == jobId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new Proposal
            {
                Id = p.Id,
                JobId = p.JobId,
                FreelancerProfileId = p.FreelancerProfileId,
                BidAmount = p.BidAmount,
                DeliveryDays = p.DeliveryDays,
                CoverLetter = p.CoverLetter,
                AttachmentUrl = p.AttachmentUrl,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> AcceptProposalAsync(int proposalId, int clientUserId)
    {
        var proposal = await _context.Proposals
            .Include(p => p.Job)
            .FirstOrDefaultAsync(p => p.Id == proposalId);

        if (proposal == null || proposal.Job == null)
            return (false, "Báo giá không tồn tại.");


        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.UserId == clientUserId);

        if (userProfile == null || proposal.Job.ClientProfileId != userProfile.Id)
            return (false, "Bạn không có quyền duyệt báo giá cho dự án này.");

        if (proposal.Job.Status != "Open")
            return (false, "Dự án này đã được giao hoặc không còn nhận duyệt.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {

            proposal.Status = "Accepted";
            proposal.UpdatedAt = DateTime.UtcNow;

            proposal.Job.Status = "InProgress";
            proposal.Job.UpdatedAt = DateTime.UtcNow;

            var otherProposals = await _context.Proposals
                .Where(p => p.JobId == proposal.JobId && p.Id != proposalId && p.Status == "Pending")
                .ToListAsync();

            foreach (var p in otherProposals)
            {
                p.Status = "Rejected";
                p.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Đã chọn ứng viên thành công!");
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "Đã xảy ra lỗi trong quá trình xử lý.");
        }
    }
}