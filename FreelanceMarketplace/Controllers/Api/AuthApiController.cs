using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ISignInService _signInService;

    public AuthApiController(IAuthService authService, ISignInService signInService)
    {
        _authService = authService;
        _signInService = signInService;
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { message = "Email không được để trống" });
        }

        bool isTaken = await _authService.IsEmailTakenAsync(email);
        return Ok(new
        {
            isTaken = isTaken,
            message = isTaken ? "Email này đã được sử dụng" : string.Empty
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, errorMessage) = await _authService.RegisterAsync(model);
        if (!success)
        {
            return Conflict(new { message = errorMessage });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Tạo tài khoản thành công",
            email = model.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (success, user, errorField, errorMessage) = await _authService.LoginAsync(model);
        if (!success)
        {
            return BadRequest(new { field = errorField, message = errorMessage });
        }

        await _signInService.SignInUserAsync(user!, model.RememberMe);
        return Ok(new { message = "Đăng nhập thành công" });
    }
}