namespace FreelanceMarketplace.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(IWebHostEnvironment env, ILogger<LocalFileStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }
    }
}
