using FreelanceMarketplace.Services;
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

    public class SubmitProposalRequest
    {
        public int JobId { get; set; }
        public int FreelancerUserId { get; set; }
        public decimal BidAmount { get; set; }
        public int DeliveryDays { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitProposalRequest request)
    {
        var result = await _proposalService.SubmitProposalAsync(
            request.JobId,
            request.FreelancerUserId,
            request.BidAmount,
            request.DeliveryDays,
            request.CoverLetter
        );

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Message });

        return Ok(new { success = true, message = result.Message });
    }

    public class AcceptProposalRequest
    {
        public int ProposalId { get; set; }
        public int ClientUserId { get; set; }
    }

    [HttpPost("accept")]
    public async Task<IActionResult> Accept([FromBody] AcceptProposalRequest request)
    {
        var result = await _proposalService.AcceptProposalAsync(request.ProposalId, request.ClientUserId);

        if (!result.Success)
            return BadRequest(new { success = false, message = result.Message });

        return Ok(new { success = true, message = result.Message });
    }

    [HttpGet("job/{jobId}")]
    public async Task<IActionResult> GetByJob(int jobId, [FromQuery] int clientUserId)
    {
        var proposals = await _proposalService.GetProposalsByJobIdAsync(jobId, clientUserId);
        return Ok(proposals);
    }
}