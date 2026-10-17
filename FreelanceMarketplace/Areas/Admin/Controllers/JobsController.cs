using FreelanceMarketplace.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class JobsController : Controller
    {
        private readonly AppDbContext _context;

        public JobsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Jobs
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.ClientProfile)
                    .ThenInclude(cp => cp.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(j => j.Status == status);
            }

            var jobs = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(jobs);
        }

        // GET: /Admin/Jobs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Skills)
                .Include(j => j.ClientProfile)
                    .ThenInclude(cp => cp.User)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: /Admin/Jobs/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                job.Status = "Approved"; // hoặc "Active" theo quy ước enum/chuỗi của bạn
                job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Jobs/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                job.Status = "Rejected";
                job.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}