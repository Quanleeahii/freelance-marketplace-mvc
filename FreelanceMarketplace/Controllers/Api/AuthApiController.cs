using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
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
            return BadRequest(new { message = "Email không được để trống." });
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
            message = "Tạo tài khoản thành công!",
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

        var result = await _authService.LoginAsync(model);
        if (!result)
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác." });
        }

        return Ok(new { message = "Đăng nhập thành công!" });
    }
}
