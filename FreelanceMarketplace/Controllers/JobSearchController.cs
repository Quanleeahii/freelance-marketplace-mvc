using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

[Route("jobs")]
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

    [HttpGet("", Name = "JobList")]
    public async Task<IActionResult> Index([FromQuery] JobFilterRequest request)
    {
        var result = await _jobSearchService.GetJobsAsync(request);
        await LoadSidebarDataAsync(request.CategoryId);
        ViewBag.Filter = request;
        return View("~/Views/Job/SearchJob.cshtml", result);
    }

    [HttpGet("{id:int}", Name = "JobDetails")]
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
    }
}