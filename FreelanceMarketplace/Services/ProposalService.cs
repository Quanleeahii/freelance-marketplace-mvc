using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class ProposalService : IProposalService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public ProposalService(AppDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<(bool Success, string ErrorMessage)> SubmitProposalAsync(SubmitProposalRequest model, int freelancerUserId)
    {
        var job = await _context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == model.JobId);

        if (job == null)
            return (false, "Công việc này không tồn tại");

        if (job.Status != "Open")
            return (false, "Công việc này đã đóng hoặc không còn nhận chào giá");

        var userProfile = await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == freelancerUserId);

        if (userProfile == null
            || string.IsNullOrWhiteSpace(userProfile.FullName)
            || string.IsNullOrWhiteSpace(userProfile.Phone)
            || string.IsNullOrWhiteSpace(userProfile.Address)
            || string.IsNullOrWhiteSpace(userProfile.City))
        {
            return (false, "Bạn cần hoàn thiện đầy đủ thông tin cá nhân trước khi gửi báo giá");
        }

        var freelancerProfile = await _context.FreelancerProfiles
            .FirstOrDefaultAsync(f => f.UserId == freelancerUserId);

        if (freelancerProfile == null)
        {
            freelancerProfile = new FreelancerProfile
            {
                UserId = freelancerUserId
            };
            _context.FreelancerProfiles.Add(freelancerProfile);
            await _context.SaveChangesAsync();
        }

        var clientProfile = await _context.ClientProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == freelancerUserId);

        if (clientProfile != null && job.ClientProfileId == clientProfile.Id)
            return (false, "Bạn không thể gửi báo giá cho dự án của chính mình");

        bool alreadyBid = await _context.Proposals
            .AnyAsync(p => p.JobId == model.JobId && p.FreelancerProfileId == freelancerProfile.Id);

        if (alreadyBid)
            return (false, "Bạn đã gửi báo giá cho dự án này rồi");

        string? attachmentUrl = null;
        if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".zip", ".png", ".jpg", ".jpeg" };
            attachmentUrl = await _fileStorage.SaveFileAsync(model.AttachmentFile, "attachments", allowedExtensions);
        }

        var proposal = new Proposal
        {
            JobId = model.JobId,
            FreelancerProfileId = freelancerProfile.Id,
            BidAmount = model.BidAmount,
            DeliveryDays = model.DeliveryDays,
            CoverLetter = model.CoverLetter?.Trim() ?? string.Empty,
            PhoneNumber = model.PhoneNumber.Trim(),
            AttachmentUrl = attachmentUrl,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> UpdateProposalAsync(SubmitProposalRequest model, int freelancerUserId)
    {
        var freelancerProfile = await _context.FreelancerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.UserId == freelancerUserId);

        if (freelancerProfile == null)
            return (false, "Không tìm thấy thông tin hồ sơ của bạn");

        var proposal = await _context.Proposals
            .FirstOrDefaultAsync(p => p.Id == model.Id && p.FreelancerProfileId == freelancerProfile.Id);

        if (proposal == null)
            return (false, "Bài chào giá không tồn tại");

        if (proposal.Status != "Pending")
            return (false, "Chỉ được phép chỉnh sửa báo giá khi dự án đang ở trạng thái chờ duyệt");

        proposal.BidAmount = model.BidAmount;
        proposal.DeliveryDays = model.DeliveryDays;
        proposal.CoverLetter = model.CoverLetter?.Trim() ?? string.Empty;
        proposal.PhoneNumber = model.PhoneNumber.Trim();
        proposal.UpdatedAt = DateTime.UtcNow;

        if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
        {
            _fileStorage.DeleteFile(proposal.AttachmentUrl);
            var allowedExtensions = new[] { ".pdf", ".docx", ".zip", ".png", ".jpg", ".jpeg" };
            proposal.AttachmentUrl = await _fileStorage.SaveFileAsync(model.AttachmentFile, "attachments", allowedExtensions);
        }
        else if (model.RemoveCurrentAttachment)
        {
            _fileStorage.DeleteFile(proposal.AttachmentUrl);
            proposal.AttachmentUrl = null;
        }

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<SubmitProposalRequest?> GetProposalForEditAsync(int proposalId, int freelancerUserId)
    {
        var freelancerProfile = await _context.FreelancerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.UserId == freelancerUserId);

        if (freelancerProfile == null)
            return null;

        var proposal = await _context.Proposals
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == proposalId && p.FreelancerProfileId == freelancerProfile.Id);

        if (proposal == null)
            return null;

        return new SubmitProposalRequest
        {
            Id = proposal.Id,
            JobId = proposal.JobId,
            BidAmount = proposal.BidAmount,
            DeliveryDays = proposal.DeliveryDays,
            CoverLetter = proposal.CoverLetter,
            PhoneNumber = proposal.PhoneNumber,
            CurrentAttachmentUrl = proposal.AttachmentUrl
        };
    }
}