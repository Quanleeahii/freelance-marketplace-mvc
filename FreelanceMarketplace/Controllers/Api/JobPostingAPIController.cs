using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers.Api;

[ApiController]
[Route("api/job-posting")]
public class JobPostingAPIController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobPostingAPIController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet("verify-budget")]
    public IActionResult VerifyBudget([FromQuery] decimal? budgetMin, [FromQuery] decimal? budgetMax)
    {
        if (budgetMin.HasValue && budgetMin.Value < 200000)
        {
            return BadRequest(new { field = "BudgetMin", message = "Ngân sách tối thiểu từ 200.000 VNĐ trở lên" });
        }

        if (budgetMax.HasValue && budgetMax.Value < 200000)
        {
            return BadRequest(new { field = "BudgetMax", message = "Ngân sách tối đa từ 200.000 VNĐ trở lên" });
        }

        if (budgetMin.HasValue && budgetMax.HasValue && budgetMax.Value <= budgetMin.Value)
        {
            return BadRequest(new { field = "BudgetMax", message = "Ngân sách tối đa phải lớn hơn ngân sách tối thiểu" });
        }

        return Ok(new { valid = true });
    }

    [HttpGet("subcategories/{parentId:int}")]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var subCategories = await _jobService.GetSubCategoriesByParentIdAsync(parentId);
        return Ok(subCategories);
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateJob([FromBody] JobCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var clientProfileId = HttpContext.Session.GetInt32("ClientProfileId");
        if (clientProfileId == null)
        {
            return Unauthorized(new { message = "Vui lòng đăng nhập để đăng bài" });
        }
        var createdJob = await _jobService.CreateJobAsync(model, clientProfileId.Value, model.SkillIds);
        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Đăng dự án thành công",
            jobId = createdJob.Id
        });
    }
}