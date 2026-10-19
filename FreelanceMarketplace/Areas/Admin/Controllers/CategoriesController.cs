using FreelanceMarketplace.Areas.Admin.Services;
using FreelanceMarketplace.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/categories")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly IAdminCategoryService _adminCategoryService;

        public CategoriesController(IAdminCategoryService adminCategoryService)
        {
            _adminCategoryService = adminCategoryService;
        }

        // GET: /admin/categories
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = new AdminCategoryViewModel
            {
                ParentCategoriesWithSubs = await _adminCategoryService.GetAllCategoriesWithHierarchyAsync(),
                ParentListForDropdown = await _adminCategoryService.GetParentCategoriesAsync()
            };

            return View(model);
        }

        // POST: /admin/categories/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string? slug, int? parentId)
        {
            var result = await _adminCategoryService.CreateCategoryAsync(name, slug, parentId);

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

        // POST: /admin/categories/delete/5
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _adminCategoryService.DeleteCategoryAsync(id);

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