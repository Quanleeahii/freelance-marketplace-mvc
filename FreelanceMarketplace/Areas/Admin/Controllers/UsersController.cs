using FreelanceMarketplace.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IAdminUserService _adminUserService;

        public UsersController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        // GET: /admin/users
        [HttpGet("")]
        public async Task<IActionResult> Index(bool? isVerified, string? search, int page = 1)
        {
            var model = await _adminUserService.GetUsersAsync(isVerified, search, page);

            ViewBag.CurrentVerified = isVerified;
            ViewBag.SearchTerm = search;

            return View(model);
        }

        // GET: /admin/users/details/5
        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _adminUserService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /admin/users/toggle-status/5
        [HttpPost("toggle-status/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _adminUserService.ToggleUserStatusAsync(id);
            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Cập nhật trạng thái tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}