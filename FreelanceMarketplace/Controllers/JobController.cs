using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreelanceMarketplace.Controllers;

public class JobController : Controller
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var clientProfileId = HttpContext.Session.GetInt32("ClientProfileId");
        if (clientProfileId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        await LoadFormDataAsync();
        return View(new JobCreateViewModel());
    }
    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyBudget(decimal? budgetMax, decimal? budgetMin)
    {
        if (budgetMax.HasValue && budgetMax.Value < 200000)
        {
            return Json("Ngân sách tối đa từ 200.000 VNĐ trở lên");
        }
        if (budgetMax.HasValue && budgetMin.HasValue && budgetMax.Value <= budgetMin.Value)
        {
            return Json("Ngân sách tối đa phải lớn hơn ngân sách tối thiểu");
        }
        return Json(true);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobCreateViewModel model)
    {
        var clientProfileId = HttpContext.Session.GetInt32("ClientProfileId");
        if (clientProfileId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync(model.ParentCategoryId);
            return View(model);
        }
        var createdJob = await _jobService.CreateJobAsync(model, clientProfileId.Value, model.SkillIds);
        return RedirectToAction("Index", "Home");
    }

    private async Task LoadFormDataAsync(int? selectedParentId = null)
    {
        var parentCategories = await _jobService.GetParentCategoriesAsync();
        var skills = await _jobService.GetAllSkillsAsync();
        ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name", selectedParentId);
        ViewBag.Skills = skills;
    }
}