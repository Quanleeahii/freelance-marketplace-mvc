using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;

namespace FreelanceMarketplace.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == request.ClientUserId);

            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    UserId = request.ClientUserId,
                    FullName = "Client Test",
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();
            }

            var categoryId = request.CategoryId ?? (await _context.Set<Category>().Select(c => c.Id).FirstOrDefaultAsync());
            if (categoryId == 0) categoryId = 1;

            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                BudgetMin = request.Budget,
                ClientProfileId = userProfile.Id,
                CategoryId = categoryId,
                Status = "Open",
                WorkType = "Remote",
                PaymentType = "Fixed",
                CreatedAt = DateTime.UtcNow
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Tạo công việc thành công!",
                jobId = job.Id
            });
        }
    }

    public class CreateJobRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public int ClientUserId { get; set; }
        public int? CategoryId { get; set; }
    }
}