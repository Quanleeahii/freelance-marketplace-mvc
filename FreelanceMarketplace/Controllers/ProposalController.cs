using FreelanceMarketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        ViewBag.JobId = jobId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int jobId, decimal bidAmount, int deliveryDays, string coverLetter)
    {
        if (bidAmount <= 0 || deliveryDays <= 0 || string.IsNullOrWhiteSpace(coverLetter))
        {
            TempData["ErrorMessage"] = "Dữ liệu nhập vào chưa hợp lệ.";
            return RedirectToAction("Details", "Job", new { id = jobId });
        }

        int currentUserId = GetCurrentUserId();
        var result = await _proposalService.SubmitProposalAsync(jobId, currentUserId, bidAmount, deliveryDays, coverLetter);

        if (result.Success)
            TempData["SuccessMessage"] = result.Message;
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Details", "Job", new { id = jobId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int proposalId, int jobId)
    {
        int currentUserId = GetCurrentUserId();
        var result = await _proposalService.AcceptProposalAsync(proposalId, currentUserId);

        if (result.Success)
            TempData["SuccessMessage"] = result.Message;
        else
            TempData["ErrorMessage"] = result.Message;

        return RedirectToAction("Details", "Job", new { id = jobId });
    }
}