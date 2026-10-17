//using FreelanceMarketplace.Data;
//using FreelanceMarketplace.Models;
//using FreelanceMarketplace.ViewModels;
//using Microsoft.EntityFrameworkCore;

//namespace FreelanceMarketplace.Services
//{
//    public class UserProfileService : IUserProfileService
//    {
//        private readonly AppDbContext _context;
//        private readonly IFileStorageService _fileStorageService;

//        public UserProfileService(AppDbContext context, IFileStorageService fileStorageService)
//        {
//            _context = context;
//            //_fileStorageService = fileStorageService;
//        }

//        public async Task<UserProfileViewModel?> GetProfileAsync(int userId)
//        {
//            var user = await _context.Users
//                .AsNoTracking()
//                .Include(u => u.UserProfile)
//                .FirstOrDefaultAsync(u => u.Id == userId);

//            if (user == null) return null;

//            return new UserProfileViewModel
//            {
//                UserId = user.Id,
//                Email = user.Email,
//                FullName = user.UserProfile?.FullName,
//                Phone = user.UserProfile?.Phone,
//                City = user.UserProfile?.City,
//                Address = user.UserProfile?.Address,
//                AvatarUrl = user.UserProfile?.AvatarUrl,
//                UpdatedAt = user.UserProfile?.UpdatedAt
//            };
//        }

//        public async Task<bool> UpdateBasicInfoAsync(int userId, UpdateBasicInfoRequest request)
//        {
//            var user = await _context.Users
//                .Include(u => u.UserProfile)
//                .FirstOrDefaultAsync(u => u.Id == userId);

//            if (user == null)
//                throw new KeyNotFoundException("Không tìm thấy thông tin tài khoản người dùng.");

//            // Kiểm tra tính duy nhất của Email khi có thay đổi
//            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
//            {
//                var isEmailTaken = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != userId);
//                if (isEmailTaken)
//                    throw new InvalidOperationException("Địa chỉ email đã được sử dụng bởi tài khoản khác.");

//                user.Email = request.Email;
//            }

//            // Khởi tạo profile nếu chưa tồn tại
//            if (user.UserProfile == null)
//            {
//                user.UserProfile = new UserProfile
//                {
//                    UserId = user.Id,
//                    CreatedAt = DateTime.UtcNow
//                };
//                _context.UserProfiles.Add(user.UserProfile);
//            }

//            // Upload ảnh đại diện nếu có tệp mới
//            //if (request.AvatarFile != null && request.AvatarFile.Length > 0)
//            //{
//            //    var newAvatarPath = await _fileStorageService.SaveImageAsync(request.AvatarFile, "avatars", maxSizeBytes: 1 * 1024 * 1024);

//            //    // Dọn dẹp ảnh cũ nếu đã có từ trước
//            //    if (!string.IsNullOrEmpty(user.UserProfile.AvatarUrl))
//            //    {
//            //        _fileStorageService.DeleteFile(user.UserProfile.AvatarUrl);
//            //    }

//            //    user.UserProfile.AvatarUrl = newAvatarPath;
//            //}

//            // Cập nhật thông tin profile
//            user.UserProfile.FullName = request.FullName;
//            user.UserProfile.Phone = request.Phone;
//            user.UserProfile.City = request.City;
//            user.UserProfile.Address = request.Address;
//            user.UserProfile.UpdatedAt = DateTime.UtcNow;
//            user.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}