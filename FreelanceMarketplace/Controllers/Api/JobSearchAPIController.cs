using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers.Api;

[ApiController]
[Route("api/jobs")]
public class JobSearchAPIController : ControllerBase
{
    private readonly IJobSearchService _jobSearchService;

    public JobSearchAPIController(IJobSearchService jobSearchService)
    {
        _jobSearchService = jobSearchService;
    }
    [HttpGet]
    public async Task<IActionResult> GetJobs([FromQuery] JobFilterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _jobSearchService.GetJobsAsync(request);
        return Ok(new
        {
            success = true,
            data = result.Items,
            meta = new
            {
                totalItems = result.TotalCount,
                currentPage = result.CurrentPage,
                pageSize = result.PageSize,
                totalPages = result.TotalPages,
                hasPreviousPage = result.HasPreviousPage,
                hasNextPage = result.HasNextPage
            }
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetJobById(int id)
    {
        var job = await _jobSearchService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy dự án phù hợp" });
        }

        return Ok(new { success = true, data = job });
    }
}