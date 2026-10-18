using System.Security.Claims;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers.Api;

[ApiController]
[Route("api/user-profile")]
[Authorize]
public class UserProfileApiController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ISignInService _signInService;

    public UserProfileApiController(IUserProfileService userProfileService, ISignInService signInService)
    {
        _userProfileService = userProfileService;
        _signInService = signInService;
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = 0;
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? HttpContext.Session.GetString("UserId");

        return int.TryParse(userIdStr, out userId) && userId > 0;
    }

    [HttpPut("basic")]
    public async Task<IActionResult> UpdateBasicInfo([FromBody] UpdateBasicInfoRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryGetCurrentUserId(out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _userProfileService.UpdateBasicInfoAsync(userId, request);
            await _signInService.RefreshSignInAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateAvatar(IFormFile file)
    {
        if (!TryGetCurrentUserId(out int userId))
        {
            return Unauthorized();
        }

        try
        {
            var avatarUrl = await _userProfileService.UpdateAvatarAsync(userId, file);
            await _signInService.RefreshSignInAsync(userId);
            return Ok(new { avatarUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("freelancer")]
    public async Task<IActionResult> UpdateFreelancerProfile([FromBody] UpdateFreelancerProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryGetCurrentUserId(out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _userProfileService.UpdateFreelancerProfileAsync(userId, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("client")]
    public async Task<IActionResult> UpdateClientProfile([FromBody] UpdateClientProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryGetCurrentUserId(out int userId))
        {
            return Unauthorized();
        }

        try
        {
            await _userProfileService.UpdateClientProfileAsync(userId, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}