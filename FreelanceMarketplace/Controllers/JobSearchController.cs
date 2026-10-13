using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

public class JobSearchController : Controller
{
    private readonly IJobSearchService _jobSearchService;
    private readonly IJobService _jobService;
    public JobSearchController(
        IJobSearchService jobSearchService,
        IJobService jobService)
    {
        _jobSearchService = jobSearchService;
        _jobService = jobService;
    }

    [HttpGet]
    [Route("jobs")]
    [Route("jobs/search")]
    public async Task<IActionResult> Index([FromQuery] JobFilterRequest request)
    {
        var result = await _jobSearchService.GetJobsAsync(request);
        await LoadSidebarDataAsync(request.CategoryId);
        ViewBag.Filter = request;
        return View(result);
    }

    [HttpGet]
    [Route("jobs/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobSearchService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }
        return View(job);
    }

    private async Task LoadSidebarDataAsync(int? selectedCategoryId = null)
    {
        var parentCategories = await _jobService.GetParentCategoriesAsync();
        var skills = await _jobService.GetAllSkillsAsync();
        ViewBag.Categories = parentCategories;
        ViewBag.Skills = skills;
        if (selectedCategoryId.HasValue && selectedCategoryId.Value > 0)
        {
            ViewBag.SubCategories = await _jobService.GetSubCategoriesByParentIdAsync(selectedCategoryId.Value);
        }
    }
}