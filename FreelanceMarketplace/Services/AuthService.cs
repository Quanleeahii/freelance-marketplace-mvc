using FreelanceMarketplace.Data;

namespace FreelanceMarketplace.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
}