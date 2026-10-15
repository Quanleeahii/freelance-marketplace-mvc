using System.Security.Claims;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

[Authorize]
public class ProposalController : Controller
{
    private readonly IProposalService _proposalService;

    public ProposalController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out int id) ? id : 0;
    }

    [HttpGet]
    public IActionResult Create(int jobId)
    {
        if (jobId <= 0) return BadRequest();

        var model = new SubmitProposalViewModel
        {
            JobId = jobId
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0) return BadRequest();
        int currentUserId = GetCurrentUserId();
        var model = await _proposalService.GetProposalForEditAsync(id, currentUserId);
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
}