using FreelanceMarketplace.Services;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            ModelState.AddModelError(string.Empty, errorMessage);
            return View(model);
        }
        return RedirectToAction("Index", "Home");
    }
}