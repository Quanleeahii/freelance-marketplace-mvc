using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Services;

public interface IProposalService
{
    Task<(bool Success, string Message)> SubmitProposalAsync(
        int jobId,
        int freelancerUserId,
        decimal bidAmount,
        int deliveryDays,
        string coverLetter,
        string? attachmentUrl = null
    );

    Task<List<Proposal>> GetProposalsByJobIdAsync(int jobId, int clientUserId);

    Task<(bool Success, string Message)> AcceptProposalAsync(int proposalId, int clientUserId);
}