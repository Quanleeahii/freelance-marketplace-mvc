using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FreelanceMarketplace.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment env, ILogger<LocalFileStorageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder, string[]? allowedExtensions = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File rỗng hoặc không tồn tại.", nameof(file));

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (allowedExtensions != null && allowedExtensions.Length > 0)
        {
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Định dạng file không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowedExtensions)}");
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{subFolder}/{uniqueFileName}";
    }

    public void DeleteFile(string? relativeFilePath)
    {
        if (string.IsNullOrWhiteSpace(relativeFilePath))
            return;

        if (relativeFilePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            relativeFilePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            var cleanPath = relativeFilePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.GetFullPath(Path.Combine(_env.WebRootPath, cleanPath));

            var uploadsFolder = Path.GetFullPath(Path.Combine(_env.WebRootPath, "uploads"));
            if (!physicalPath.StartsWith(uploadsFolder, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa file vật lý: {Path}", relativeFilePath);
        }
    }
}