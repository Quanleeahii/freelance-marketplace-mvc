using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using Microsoft.AspNetCore.Mvc;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Controllers.Api;

[ApiController]
[Route("api/jobs")] 
public class JobPostingAPIController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobPostingAPIController(IJobService jobService)
    {
        _jobService = jobService;
    }

    
    [HttpPost("create")] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateJob([FromBody] JobCreateViewModel model, [FromQuery] List<int>? skillIds = null)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdJob = await _jobService.CreateJobAsync(model, skillIds);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Đăng dự án thành công.",
            data = createdJob
        });
    }
}