using SmartRecruitment_Project.DTOs.Employers;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IEmployerService
    {
        Task<EmployerProfileDto> GetMyProfileAsync(int userId);

        Task<EmployerProfileDto> CreateOrUpdateProfileAsync(
            int userId,
            UpdateEmployerProfileDto dto);
    }
}