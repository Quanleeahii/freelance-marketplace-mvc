using FreelanceMarketplace.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/skills")]
    [Authorize(Roles = "Admin")]
    public class SkillsController : Controller
    {
        private readonly IAdminSkillService _adminSkillService;

        public SkillsController(IAdminSkillService adminSkillService)
        {
            _adminSkillService = adminSkillService;
        }

        // GET: /admin/skills
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var skills = await _adminSkillService.GetAllSkillsAsync();
            return View(skills);
        }

        // POST: /admin/skills/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string? slug)
        {
            var result = await _adminSkillService.CreateSkillAsync(name, slug);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /admin/skills/delete/5
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminSkillService.DeleteSkillAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}