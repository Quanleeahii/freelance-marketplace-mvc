using FreelanceMarketplace.Data;
using FreelanceMarketplace.Models;
using FreelanceMarketplace.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FreelanceMarketplace.Services;

public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public UserProfileService(AppDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<ManageProfileViewModel> GetManageProfileAsync(int userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserProfile)
            .Include(u => u.FreelancerProfile)
            .Include(u => u.ClientProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        return new ManageProfileViewModel
        {
            BasicInfo = new UserProfileViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.UserProfile?.FullName,
                Phone = user.UserProfile?.Phone,
                City = user.UserProfile?.City,
                Address = user.UserProfile?.Address,
                AvatarUrl = user.UserProfile?.AvatarUrl,
                IsVerified = user.IsVerified,
                UpdatedAt = user.UserProfile?.UpdatedAt
            },
            FreelancerInfo = new FreelancerProfileViewModel
            {
                CategoryId = user.FreelancerProfile?.CategoryId,
                Bio = user.FreelancerProfile?.Bio,
                PersonalWebsite = user.FreelancerProfile?.PersonalWebsite,
                ExperienceLevel = user.FreelancerProfile?.ExperienceLevel,
                WorkType = user.FreelancerProfile?.WorkType,
                IsAvailable = user.FreelancerProfile?.IsAvailable ?? false
            },
            ClientInfo = new ClientProfileViewModel
            {
                DisplayName = user.ClientProfile?.DisplayName,
                Website = user.ClientProfile?.Website,
                Description = user.ClientProfile?.Description
            }
        };
    }

    public async Task<bool> UpdateBasicInfoAsync(int userId, UpdateBasicInfoRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy tài khoản người dùng.");
        }

        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserProfiles.Add(user.UserProfile);
        }

        user.UserProfile.FullName = request.FullName.Trim();
        user.UserProfile.Phone = request.Phone?.Trim();
        user.UserProfile.City = request.City?.Trim();
        user.UserProfile.Address = request.Address?.Trim();

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            user.UserProfile.AvatarUrl = request.AvatarUrl;
        }

        user.UserProfile.UpdatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> UpdateAvatarAsync(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Vui lòng chọn file ảnh hợp lệ", nameof(file));
        }

        if (file.Length > 1 * 1024 * 1024)
        {
            throw new ArgumentException("Kích thước ảnh không được vượt quá 1MB");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Chỉ chấp nhận các định dạng ảnh: .jpg, .jpeg, .png, .webp");
        }

        var user = await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy tài khoản người dùng");
        }

        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserProfiles.Add(user.UserProfile);
        }

        if (!string.IsNullOrWhiteSpace(user.UserProfile.AvatarUrl))
        {
            _fileStorageService.DeleteFile(user.UserProfile.AvatarUrl);
        }

        var newAvatarUrl = await _fileStorageService.SaveFileAsync(file, "avatars", allowedExtensions);

        user.UserProfile.AvatarUrl = newAvatarUrl;
        user.UserProfile.UpdatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return newAvatarUrl;
    }

    public async Task<bool> UpdateFreelancerProfileAsync(int userId, UpdateFreelancerProfileRequest request)
    {
        var profile = await _context.FreelancerProfiles.FirstOrDefaultAsync(f => f.UserId == userId);

        if (profile == null)
        {
            profile = new FreelancerProfile
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.FreelancerProfiles.Add(profile);
        }

        profile.CategoryId = request.CategoryId;
        profile.Bio = request.Bio?.Trim();
        profile.PersonalWebsite = request.PersonalWebsite?.Trim();
        profile.ExperienceLevel = request.ExperienceLevel;
        profile.WorkType = request.WorkType;
        profile.IsAvailable = request.IsAvailable;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateClientProfileAsync(int userId, UpdateClientProfileRequest request)
    {
        var profile = await _context.ClientProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

        if (profile == null)
        {
            profile = new ClientProfile
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.ClientProfiles.Add(profile);
        }

        profile.DisplayName = request.DisplayName?.Trim();
        profile.Website = request.Website?.Trim();
        profile.Description = request.Description?.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}