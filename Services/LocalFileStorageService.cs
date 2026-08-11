using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storagePath;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _storagePath = Path.Combine(
                environment.ContentRootPath,
                "ProtectedStorage",
                "CVs");

            Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> SaveFileAsync(
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(file.FileName);

            var storedFileName =
                $"{Guid.NewGuid():N}{extension}";

            var fullPath =
                Path.Combine(_storagePath, storedFileName);

            await using var stream =
                new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write);

            await file.CopyToAsync(
                stream,
                cancellationToken);

            return storedFileName;
        }

        public Task DeleteFileAsync(string storedFileName)
        {
            var fullPath =
                Path.Combine(_storagePath, storedFileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<Stream> OpenFileAsync(string storedFileName)
        {
            var fullPath =
                Path.Combine(_storagePath, storedFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    "The requested CV file was not found.");
            }

            Stream stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return Task.FromResult(stream);
        }
    }
}
