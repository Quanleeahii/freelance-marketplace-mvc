using FreelanceMarketplace.Areas.Admin.Models;
using FreelanceMarketplace.Data;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FreelanceMarketplace.Enums;
namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserProfileService _userProfileService;

        public HomeController(AppDbContext context, IUserProfileService userProfileService)
        {
            _context = context;
            _userProfileService = userProfileService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                PendingJobsCount = await _context.Jobs.CountAsync(j => j.Status == JobStatusConstants.Open),
                TotalJobsCount = await _context.Jobs.CountAsync(),
                VerifiedUsersCount = await _context.Users.CountAsync(u => u.Role != "Admin" && u.IsVerified),
                InProgressJobsCount = await _context.Jobs.CountAsync(j => j.Status == JobStatusConstants.InProgress),
                TotalUsersCount = await _context.Users.CountAsync(u => u.Role != "Admin")
            };

            return View(viewModel);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profileData = await _userProfileService.GetManageProfileAsync(userId);
            return View(profileData.BasicInfo);
        }

        [HttpPost("profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateBasicInfoRequest request, IFormFile? avatarFile)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var avatarUrl = await _userProfileService.UpdateAvatarAsync(userId, avatarFile);
                request.AvatarUrl = avatarUrl;
            }

            await _userProfileService.UpdateBasicInfoAsync(userId, request);

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Profile));
        }
    }
}