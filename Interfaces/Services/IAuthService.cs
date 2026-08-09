using SmartRecruitment_Project.DTOs.Auth;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterJobSeekerAsync(
            JobSeekerRegisterDto dto);

        Task<AuthResponseDto> RegisterEmployerAsync(
            EmployerRegisterDto dto);

        Task<AuthResponseDto> LoginAsync(
            LoginDto dto);
    }
}