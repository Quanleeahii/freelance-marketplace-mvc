using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using Microsoft.AspNetCore.Mvc;
using FreelanceMarketplace.ViewModels;

namespace FreelanceMarketplace.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
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

        var job = new Job
        {
            ClientProfileId = model.ClientProfileId > 0 ? model.ClientProfileId : 1,
            CategoryId = model.CategoryId,
            Title = model.Title,
            Description = model.Description,

            WorkType = model.WorkType ?? string.Empty,       // Nếu null thì gán chuỗi rỗng
            PaymentType = model.PaymentType ?? string.Empty, // Nếu null thì gán chuỗi rỗng
            LocationCity = model.LocationCity ?? string.Empty,

            BudgetMin = model.BudgetMin ?? 0m,               // Thêm ?? 0m

            Deadline = model.Deadline
        };

        var createdJob = await _jobService.CreateJobAsync(job, skillIds);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Đăng dự án thành công.",
            data = createdJob
        });
    }
}