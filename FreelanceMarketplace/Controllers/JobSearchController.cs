using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;


public class JobSearchController : Controller
{
    private readonly IJobSearchService _jobSearchService;

    public JobSearchController(IJobSearchService jobSearchService)
    {
        _jobSearchService = jobSearchService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(JobSearchViewModel model)
    {
        var (items, totalItems) = await _jobSearchService.GetJobsAsync(model);

        ViewBag.TotalItems = totalItems;
        ViewBag.SearchModel = model;

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobSearchService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        return View(job);
    }
}