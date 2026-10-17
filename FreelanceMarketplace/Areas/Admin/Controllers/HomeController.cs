using System.Security.Claims;
using FreelanceMarketplace.Areas.Admin.Models;
using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Admin/Home/Index hoặc /Admin
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                PendingJobsCount = await _context.Jobs.CountAsync(j => j.Status == "Pending" || j.Status == "WaitingApproval"),
                TotalJobsCount = await _context.Jobs.CountAsync(),
                TotalFreelancersCount = await _context.FreelancerProfiles.CountAsync(),
                TotalProposalsCount = await _context.Proposals.CountAsync(),
                TotalUsersCount = await _context.Users.CountAsync()
            };

            return View(viewModel);
        }

        // GET: /Admin/Home/Profile
        public async Task<IActionResult> Profile()
        {
            // Lấy UserId từ Cookie/Claims của user đang đăng nhập
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            User? user = null;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                user = await _context.Users
                    .Include(u => u.UserProfile)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }

            // Fallback nếu đang chạy thử hoặc chưa có phiên đăng nhập
            if (user == null)
            {
                user = await _context.Users
                    .Include(u => u.UserProfile)
                    .FirstOrDefaultAsync();
            }

            if (user == null)
            {
                return NotFound("Không tìm thấy thông tin tài khoản admin.");
            }

            return View(user);
        }

        // POST: /Admin/Home/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string phone, string city, string address, IFormFile? avatarFile)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = 1;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
            {
                userId = parsedId;
            }
            else
            {
                var defaultUser = await _context.Users.FirstOrDefaultAsync();
                if (defaultUser != null)
                {
                    userId = defaultUser.Id;
                }
            }

            // Xử lý upload ảnh đại diện nếu có file tải lên
            string? newAvatarPath = null;
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                if (allowedExtensions.Contains(ext))
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"avatar_{userId}_{Guid.NewGuid():N}{ext}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }

                    newAvatarPath = $"/uploads/avatars/{uniqueFileName}";
                }
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    UserId = userId,
                    FullName = fullName,
                    Phone = phone,
                    City = city,
                    Address = address,
                    AvatarUrl = newAvatarPath,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserProfiles.Add(userProfile);
            }
            else
            {
                userProfile.FullName = fullName;
                userProfile.Phone = phone;
                userProfile.City = city;
                userProfile.Address = address;
                if (!string.IsNullOrEmpty(newAvatarPath))
                {
                    userProfile.AvatarUrl = newAvatarPath;
                }
                userProfile.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ và ảnh đại diện thành công!";
            return RedirectToAction(nameof(Profile));
        }
    }
}