using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Interfaces.Repositories
{
    public interface IEmployerRepository
    {
        Task<EmployerProfile?> GetByUserIdAsync(int userId);

        Task<EmployerProfile> CreateAsync(
            EmployerProfile employerProfile);

        Task<EmployerProfile> UpdateAsync(
            EmployerProfile employerProfile);
    }
}