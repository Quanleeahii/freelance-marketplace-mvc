using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthApiController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { message = "Email không được để trống" });
        }

        bool isTaken = await _authService.IsEmailTakenAsync(email);
        return Ok(new { isTaken });
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

        return Ok(new { message = "Đăng nhập thành công" });
    }
}
