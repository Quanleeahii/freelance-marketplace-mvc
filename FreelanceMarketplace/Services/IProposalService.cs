using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services;

public interface IProposalService
{
    Task<(bool Success, string ErrorMessage)> SubmitProposalAsync(SubmitProposalRequest model, int freelancerUserId);
    Task<(bool Success, string ErrorMessage)> UpdateProposalAsync(SubmitProposalRequest model, int freelancerUserId);
    Task<SubmitProposalRequest?> GetProposalForEditAsync(int proposalId, int freelancerUserId);
}