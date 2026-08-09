namespace SmartRecruitment_Project.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}