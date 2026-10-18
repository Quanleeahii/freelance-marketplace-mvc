using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FreelanceMarketplace.Controllers;

[Route("jobs")]
public class JobController : Controller
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet("create", Name = "CreateJob")]
    public async Task<IActionResult> CreateJob()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? HttpContext.Session.GetString("UserId");
        //if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        //{
        //    return RedirectToAction("Login", "Account", new { returnUrl = "/Job/CreateJob" });
        //}
        //var userProfile = await _jobService.GetUserProfileByUserIdAsync(userId);

        //bool isProfileCompleted = userProfile != null
        //                          && !string.IsNullOrWhiteSpace(userProfile.FullName)
        //                          && !string.IsNullOrWhiteSpace(userProfile.Phone);
        //if (!isProfileCompleted)
        //{
        //    TempData["WarningMessage"] = "Bạn cần cập nhật đầy đủ thông tin cá nhân trước khi đăng dự án";
        //    return RedirectToAction("Profile", "Account"); 
        //}
        //await LoadFormDataAsync();
        //return View(new JobCreateViewModel());
        if (string.IsNullOrEmpty(userIdStr))
        {
            return RedirectToAction("Login", "Account", new { returnUrl = "/jobs/create" });
        }
        await LoadFormDataAsync();
        return View(new JobCreateViewModel());
    }
    private async Task LoadFormDataAsync(int? selectedParentId = null)
    {
        var parentCategories = await _jobService.GetParentCategoriesAsync();
        var skills = await _jobService.GetAllSkillsAsync();
        ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "Name", selectedParentId);
        ViewBag.Skills = skills;
    }
}