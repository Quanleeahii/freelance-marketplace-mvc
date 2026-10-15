using Microsoft.AspNetCore.Http;

namespace FreelanceMarketplace.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile file, string subFolder, long maxSizeBytes = 1048576);
        void DeleteFile(string? relativePath);
    }
}