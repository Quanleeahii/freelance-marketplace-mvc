using System.Security.Claims;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProposalApiController : ControllerBase
{
    private readonly IProposalService _proposalService;

    public ProposalApiController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    private int GetEffectiveUserId(int fallbackId)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out int id) && id > 0 ? id : fallbackId;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitProposalViewModel request)
    {
        int userId = GetEffectiveUserId(request.FreelancerUserId);
        if (userId <= 0)
            return BadRequest(new { success = false, message = "Vui lòng cung cấp FreelancerUserId hợp lệ." });

        var result = await _proposalService.SubmitProposalAsync(request, userId);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Message });

        return Ok(new { success = true, message = result.Message });
    }

    [HttpGet("my-proposals")]
    public async Task<IActionResult> GetMyProposals([FromQuery] int freelancerUserId = 0)
    {
        int userId = GetEffectiveUserId(freelancerUserId);
        if (userId <= 0)
            return BadRequest(new { success = false, message = "FreelancerUserId không hợp lệ." });

        var proposals = await _proposalService.GetMyProposalsAsync(userId);
        return Ok(proposals);
    }
}