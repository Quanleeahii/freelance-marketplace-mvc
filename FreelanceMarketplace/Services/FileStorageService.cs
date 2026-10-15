using Microsoft.AspNetCore.Http;

namespace FreelanceMarketplace.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".heif" };

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile file, string subFolder, long maxSizeBytes = 1048576)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Tệp tin rỗng hoặc không tồn tại.");

            if (file.Length > maxSizeBytes)
                throw new InvalidOperationException($"Dung lượng ảnh vượt quá giới hạn cho phép ({maxSizeBytes / (1024 * 1024)}MB).");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Định dạng ảnh không được hỗ trợ (chỉ nhận JPG, JPEG, PNG, GIF, HEIC, HEIF).");

            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var targetFolder = Path.Combine(webRoot, "uploads", subFolder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var uniqueFileName = $"{Guid.NewGuid():N}_{DateTime.UtcNow.Ticks}{extension}";
            var absolutePath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subFolder}/{uniqueFileName}";
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var absolutePath = Path.Combine(webRoot, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(absolutePath))
            {
                try
                {
                    File.Delete(absolutePath);
                }
                catch
                {
                    // Tránh crash ứng dụng nếu file đang bị lock
                }
            }
        }
    }
}