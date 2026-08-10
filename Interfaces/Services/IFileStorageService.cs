namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(
            IFormFile file,
            CancellationToken cancellationToken = default);

        Task DeleteFileAsync(string storedFileName);

        Task<Stream> OpenFileAsync(string storedFileName);
    }
}