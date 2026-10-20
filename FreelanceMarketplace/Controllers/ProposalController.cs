using FreelanceMarketplace.Services;
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

    [HttpGet("jobs/{jobId:int}/proposals/create")]
    public IActionResult Create(int jobId)
    {
        if (jobId <= 0) return BadRequest();
        return RedirectToAction("Details", "JobSearch", new { id = jobId });
    }
}