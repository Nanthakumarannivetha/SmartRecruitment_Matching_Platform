using SmartRecruitment_Project.DTOs.Employers;
using SmartRecruitment_Project.Exceptions;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;

        public EmployerService(
            IEmployerRepository employerRepository)
        {
            _employerRepository = employerRepository;
        }

        public async Task<EmployerProfileDto> GetMyProfileAsync(
            int userId)
        {
            var profile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            return MapToDto(profile);
        }

        public async Task<EmployerProfileDto> CreateOrUpdateProfileAsync(
            int userId,
            UpdateEmployerProfileDto dto)
        {
            var profile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                profile = new EmployerProfile
                {
                    UserId = userId,
                    CompanyName = dto.CompanyName.Trim(),
                    Location = dto.Location?.Trim(),
                    Description = dto.Description?.Trim(),
                    Website = dto.Website?.Trim(),
                    UpdatedAt = DateTime.UtcNow
                };

                profile =
                    await _employerRepository.CreateAsync(profile);
            }
            else
            {
                profile.CompanyName = dto.CompanyName.Trim();
                profile.Location = dto.Location?.Trim();
                profile.Description = dto.Description?.Trim();
                profile.Website = dto.Website?.Trim();
                profile.UpdatedAt = DateTime.UtcNow;

                profile =
                    await _employerRepository.UpdateAsync(profile);
            }

            return MapToDto(profile);
        }

        private static EmployerProfileDto MapToDto(
            EmployerProfile profile)
        {
            return new EmployerProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                CompanyName = profile.CompanyName,
                Location = profile.Location,
                Description = profile.Description,
                Website = profile.Website,
                UpdatedAt = profile.UpdatedAt
            };
        }
    }
}