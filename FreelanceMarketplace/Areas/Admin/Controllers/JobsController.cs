using FreelanceMarketplace.Areas.Admin.Services; // Hoặc FreelanceMarketplace.Services tùy nơi bạn đặt file
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/jobs")]
    [Authorize(Roles = "Admin")]
    public class JobsController : Controller
    {
        // 1. Đổi IJobService thành IAdminJobService
        private readonly IAdminJobService _adminJobService;

        public JobsController(IAdminJobService adminJobService)
        {
            _adminJobService = adminJobService;
        }

        // GET: /admin/jobs
        [HttpGet("")]
        public async Task<IActionResult> Index(string? status, string? keyword, int page = 1)
        {
            var model = await _adminJobService.GetJobsAsync(status, keyword, page);

            ViewBag.CurrentStatus = status;
            ViewBag.Keyword = keyword;

            return View(model);
        }

        // GET: /admin/jobs/details/5
        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _adminJobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: /admin/jobs/toggle-hide/5
        [HttpPost("toggle-hide/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHide(int id)
        {
            var success = await _adminJobService.ToggleJobStatusAsync(id);
            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Cập nhật trạng thái bài đăng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}