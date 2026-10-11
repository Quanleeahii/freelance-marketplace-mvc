using FreelanceMarketplace.Models;
using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceMarketplace.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
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
            await SignInUserAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
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

        await SignInUserAsync(user!, model.RememberMe);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
    private async Task SignInUserAsync(User user, bool isPersistent = false)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserProfile?.FullName ?? user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserRole", user.Role);
        if (user.UserProfile != null)
        {
            HttpContext.Session.SetString("FullName", user.UserProfile.FullName);
        }
    }
}