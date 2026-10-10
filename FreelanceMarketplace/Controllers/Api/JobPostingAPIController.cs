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

    /// <summary>
    /// API Đăng dự án mới dành cho Khách hàng
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateJob([FromBody] JobCreateViewModel model, [FromQuery] List<int>? skillIds = null)
    {
        // Kiểm tra tính hợp lệ của dữ liệu thông qua Data Annotations trong ViewModel
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Controller hoàn toàn không xử lý logic database, gọi thẳng xuống Service
        var createdJob = await _jobService.CreateJobAsync(model, skillIds);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Đăng dự án thành công.",
            data = createdJob
        });
    }
}