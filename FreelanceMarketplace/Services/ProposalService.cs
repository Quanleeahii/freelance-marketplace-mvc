using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class ProposalService : IProposalService
{
    private readonly AppDbContext _context;

    public ProposalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> SubmitProposalAsync(SubmitProposalViewModel model, int freelancerUserId)
    {
        var job = await _context.Jobs.FindAsync(model.JobId);
        if (job == null)
            return (false, "Công việc này không tồn tại.");

        if (job.Status != "Open")
            return (false, "Công việc này đã đóng hoặc không còn nhận chào giá.");

        var freelancerProfile = await _context.FreelancerProfiles
            .FirstOrDefaultAsync(f => f.UserId == freelancerUserId);

        if (freelancerProfile == null)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == freelancerUserId);
            if (!userExists)
                return (false, "Người dùng không tồn tại.");

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
            .AnyAsync(p => p.JobId == model.JobId && p.FreelancerProfileId == freelancerProfile.Id);

        if (alreadyBid)
            return (false, "Bạn đã gửi báo giá cho dự án này rồi.");

        var proposal = new Proposal
        {
            JobId = model.JobId,
            FreelancerProfileId = freelancerProfile.Id,
            BidAmount = model.BidAmount,
            DeliveryDays = model.DeliveryDays,
            CoverLetter = model.CoverLetter,
            AttachmentUrl = model.AttachmentUrl,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();

        return (true, "Gửi báo giá thành công!");
    }

    public async Task<List<FreelancerProposalItemViewModel>> GetMyProposalsAsync(int freelancerUserId)
    {
        var freelancerProfile = await _context.FreelancerProfiles
            .FirstOrDefaultAsync(f => f.UserId == freelancerUserId);

        if (freelancerProfile == null)
            return new List<FreelancerProposalItemViewModel>();

        return await _context.Proposals
            .AsNoTracking()
            .Where(p => p.FreelancerProfileId == freelancerProfile.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new FreelancerProposalItemViewModel
            {
                Id = p.Id,
                JobId = p.JobId,
                JobTitle = p.Job != null ? p.Job.Title : string.Empty,
                BidAmount = p.BidAmount,
                DeliveryDays = p.DeliveryDays,
                CoverLetter = p.CoverLetter,
                AttachmentUrl = p.AttachmentUrl,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }
}