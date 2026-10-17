//using System.Security.Claims;
//using FreelanceMarketplace.Services;
//using FreelanceMarketplace.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace FreelanceMarketplace.Controllers.Api
//{
//    [Route("api/user/profile")]
//    [ApiController]
//    public class UserProfileApiController : ControllerBase
//    {
//        private readonly IUserProfileService _profileService;

//        public UserProfileApiController(IUserProfileService profileService)
//        {
//            _profileService = profileService;
//        }

//        private int? GetCurrentUserId()
//        {
//            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
//                          ?? User.FindFirst("id")?.Value;

//            if (int.TryParse(idClaim, out int userId))
//            {
//                return userId;
//            }
//            return null;
//        }

//        // 1. GET: Lấy thông tin hồ sơ theo UserId
//        // GET: https://localhost:7098/api/user/profile/{userId}
//        [HttpGet("{userId:int}")]
//        public async Task<IActionResult> GetProfile(int userId)
//        {
//            var profile = await _profileService.GetProfileAsync(userId);
//            if (profile == null)
//            {
//                return NotFound(new { success = false, message = $"Không tìm thấy hồ sơ cho User ID = {userId}" });
//            }

//            return Ok(new { success = true, data = profile });
//        }

//        // 2. GET: Lấy thông tin hồ sơ của chính user đang đăng nhập (dùng Cookie)
//        // GET: https://localhost:7098/api/user/profile/me
//        [HttpGet("me")]
//        public async Task<IActionResult> GetMyProfile()
//        {
//            var userId = GetCurrentUserId();
//            if (userId == null)
//            {
//                return Unauthorized(new { success = false, message = "Chưa đăng nhập hoặc phiên đăng nhập hết hạn." });
//            }

//            var profile = await _profileService.GetProfileAsync(userId.Value);
//            if (profile == null)
//            {
//                return NotFound(new { success = false, message = "Không tìm thấy hồ sơ người dùng." });
//            }

//            return Ok(new { success = true, data = profile });
//        }

//        // 3. PUT/POST: Cập nhật thông tin cá nhân (hỗ trợ multipart/form-data upload file)
//        // POST: https://localhost:7098/api/user/profile/update/{userId}
//        [HttpPost("update/{userId:int}")]
//        [Consumes("multipart/form-data")]
//        public async Task<IActionResult> UpdateProfile(int userId, [FromForm] UpdateBasicInfoRequest request)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                await _profileService.UpdateBasicInfoAsync(userId, request);
//                return Ok(new { success = true, message = "Cập nhật thông tin cá nhân thành công!" });
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { success = false, message = ex.Message });
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(new { success = false, message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, new
//                {
//                    success = false,
//                    message = "Đã xảy ra lỗi máy chủ nội bộ.",
//                    detail = ex.Message
//                });
//            }
//        }
//    }
//}