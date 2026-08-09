using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);

        DateTime GetTokenExpiry();
    }
}