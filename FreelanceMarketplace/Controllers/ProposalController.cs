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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubmitProposalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int currentUserId = GetCurrentUserId();
        var result = await _proposalService.SubmitProposalAsync(model, currentUserId);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToAction("Details", "Job", new { id = model.JobId });
    }
}