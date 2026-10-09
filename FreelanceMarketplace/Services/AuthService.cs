using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace FreelanceMarketplace.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        string normalizedEmail = email.Trim().ToLower();
        return await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
    }

    public async Task<(bool Success, string ErrorMessage)> RegisterAsync(RegisterViewModel model)
    {
        if (await IsEmailTakenAsync(model.Email))
        {
            return (false, "Email này đã được sử dụng");
        }
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User
        {
            Email = model.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Role = "Client",
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            IsVerified = false,
            UserProfile = new UserProfile
            {
                FullName = model.FullName.Trim()
            }
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }
    public async Task<bool> LoginAsync(LoginViewModel model)
    {
        string normalizedEmail = model.Email.Trim().ToLower();
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (user == null || user.Status != "Active")
        {
            return false;
        }
        return BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
    }
    public async Task<User?> ValidateUserAsync(LoginViewModel model)
    {
        string normalizedEmail = model.Email.Trim().ToLower();
        var user = await _context.Users.Include(u => u.UserProfile).AsNoTracking().FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (user == null || user.Status != "Active")
        {
            return null;
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
        return isPasswordValid ? user : null;
    }
}