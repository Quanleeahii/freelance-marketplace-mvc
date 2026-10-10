using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Services;

public interface IProposalService
{
    Task<(bool Success, string Message)> SubmitProposalAsync(SubmitProposalViewModel model, int freelancerUserId);

    Task<List<FreelancerProposalItemViewModel>> GetMyProposalsAsync(int freelancerUserId);
}
