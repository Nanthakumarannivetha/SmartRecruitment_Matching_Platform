using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int userId);

        Task<User> UpdateUserAsync(User user);

        Task<int> GetTotalUsersAsync();

        Task<int> GetTotalVacanciesAsync();

        Task<int> GetTotalApplicationsAsync();
    }
}