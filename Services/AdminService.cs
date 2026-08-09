using SmartRecruitment_Project.DTOs.Admin;
using SmartRecruitment_Project.Exceptions;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _adminRepository.GetAllUsersAsync();

            return users
                .Select(MapToDto)
                .ToList();
        }

        public async Task<AdminUserDto> ActivateUserAsync(int userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }

            user.IsActive = true;

            await _adminRepository.UpdateUserAsync(user);

            return MapToDto(user);
        }

        public async Task<AdminUserDto> DeactivateUserAsync(int userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }

            user.IsActive = false;

            await _adminRepository.UpdateUserAsync(user);

            return MapToDto(user);
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var totalUsers =
                await _adminRepository.GetTotalUsersAsync();

            var totalVacancies =
                await _adminRepository.GetTotalVacanciesAsync();

            var totalApplications =
                await _adminRepository.GetTotalApplicationsAsync();

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalVacancies = totalVacancies,
                TotalApplications = totalApplications
            };
        }

        private static AdminUserDto MapToDto(User user)
        {
            return new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}