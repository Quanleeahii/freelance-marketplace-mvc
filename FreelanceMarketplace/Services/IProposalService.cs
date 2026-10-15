using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services;

public interface IProposalService
{
    Task<(bool Success, string ErrorMessage)> SubmitProposalAsync(SubmitProposalViewModel model, int freelancerUserId);
    Task<(bool Success, string ErrorMessage)> UpdateProposalAsync(SubmitProposalViewModel model, int freelancerUserId);
    Task<SubmitProposalViewModel?> GetProposalForEditAsync(int proposalId, int freelancerUserId);
}