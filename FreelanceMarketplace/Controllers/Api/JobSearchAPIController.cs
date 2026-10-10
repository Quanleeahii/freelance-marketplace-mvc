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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetJobs([FromQuery] JobSearchViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (items, totalItems) = await _jobSearchService.GetJobsAsync(model);
        int totalPages = (int)Math.Ceiling(totalItems / (double)model.PageSize);

        return Ok(new
        {
            success = true,
            meta = new
            {
                totalItems = totalItems,
                pageNumber = model.PageNumber,
                pageSize = model.PageSize,
                totalPages = totalPages > 0 ? totalPages : 1
            },
            data = items
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobById(int id)
    {
        var job = await _jobSearchService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { success = false, message = "Không tìm thấy dự án." });
        }

        return Ok(new { success = true, data = job });
    }
}