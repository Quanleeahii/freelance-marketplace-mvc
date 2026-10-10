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
    public async Task<IActionResult> CreateJob([FromBody] JobCreateViewModel model, [FromQuery] List<int>? skillIds)
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