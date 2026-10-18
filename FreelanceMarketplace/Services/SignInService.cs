using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FreelanceMarketplace.Services;

public class SignInService : ISignInService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;
    public SignInService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    public async Task SignInUserAsync(User user, bool isPersistent = false)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;
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

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        httpContext.Session.SetString("UserId", user.Id.ToString());
        httpContext.Session.SetString("UserEmail", user.Email);
        httpContext.Session.SetString("UserRole", user.Role);
        if (!string.IsNullOrEmpty(user.UserProfile?.FullName))
        {
            httpContext.Session.SetString("FullName", user.UserProfile.FullName);
        }
    }
    public async Task RefreshSignInAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            await SignInUserAsync(user, isPersistent: true);
        }
    }
    public async Task SignOutUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        httpContext.Session.Clear();
    }
}