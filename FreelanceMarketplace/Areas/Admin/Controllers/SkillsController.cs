using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SkillsController : Controller
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Skills
        public async Task<IActionResult> Index()
        {
            var skills = await _context.Skills
                .Include(s => s.Category)
                .OrderBy(s => s.CategoryId)
                .ThenBy(s => s.Name)
                .ToListAsync();

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(skills);
        }

        // POST: /Admin/Skills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Skill skill)
        {
            if (string.IsNullOrWhiteSpace(skill.Slug))
            {
                skill.Slug = Regex.Replace(skill.Name.ToLower().Trim(), @"\s+", "-");
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Skills/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}