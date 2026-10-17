using Microsoft.AspNetCore.Http;

namespace FreelanceMarketplace.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subFolder, string[]? allowedExtensions = null);
    void DeleteFile(string? relativeFilePath);
}