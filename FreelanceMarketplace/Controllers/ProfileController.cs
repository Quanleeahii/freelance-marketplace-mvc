using System.Security.Claims;
using FreelanceMarketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

[Authorize]
[Route("profile")]
public class ProfileController : Controller
{
    private readonly IUserProfileService _userProfileService;

    public ProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet("", Name = "ManageProfile")]
    public async Task<IActionResult> Manage()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId))
        {
            return RedirectToRoute("Login", new { returnUrl = "/profile" });
        }

        try
        {
            var model = await _userProfileService.GetManageProfileAsync(userId);
            return View(model);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Không tìm thấy thông tin tài khoản.");
        }
    }
}