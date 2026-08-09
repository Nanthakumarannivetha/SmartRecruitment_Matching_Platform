using SmartRecruitment_Project.DTOs.Admin;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IAdminService
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();

        Task<AdminUserDto> ActivateUserAsync(int userId);

        Task<AdminUserDto> DeactivateUserAsync(int userId);

        Task<AdminDashboardDto> GetDashboardAsync();
    }
}