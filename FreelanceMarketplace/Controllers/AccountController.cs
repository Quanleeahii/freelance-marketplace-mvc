using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplace.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ISignInService _signInService;

    public AccountController(IAuthService authService, ISignInService signInService)
    {
        _authService = authService;
        _signInService = signInService;
    }

    [HttpGet("/register", Name = "Register")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errorMessage) = await _authService.RegisterAsync(model);
        if (!success)
        {
            ModelState.AddModelError(nameof(model.Email), errorMessage);
            return View(model);
        }

        var user = await _authService.ValidateUserAsync(new LoginViewModel
        {
            Email = model.Email,
            Password = model.Password
        });

        if (user != null)
        {
            await _signInService.SignInUserAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        return RedirectToRoute("Login");
    }

    [HttpGet("/login", Name = "Login")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, user, errorField, errorMessage) = await _authService.LoginAsync(model);
        if (!success)
        {
            ModelState.AddModelError(errorField ?? string.Empty, errorMessage ?? "Đăng nhập không thành công");
            return View(model);
        }

        await _signInService.SignInUserAsync(user!, model.RememberMe);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        if (user!.Role == "Admin")
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("/logout", Name = "Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInService.SignOutUserAsync();
        return RedirectToAction("Index", "Home");
    }
}