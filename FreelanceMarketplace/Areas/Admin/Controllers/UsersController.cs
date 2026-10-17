using FreelanceMarketplace.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(string? role, string? search)
        {
            var query = _context.Users
                .Include(u => u.UserProfile)
                .Include(u => u.FreelancerProfile)
                .Include(u => u.ClientProfile)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                if (role == "Freelancer")
                    query = query.Where(u => u.FreelancerProfile != null);
                else if (role == "Client")
                    query = query.Where(u => u.ClientProfile != null);
            }

            var users = await query.OrderByDescending(u => u.Id).ToListAsync();

            ViewBag.CurrentRole = role;
            ViewBag.SearchTerm = search;

            return View(users);
        }

        // POST: /Admin/Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}