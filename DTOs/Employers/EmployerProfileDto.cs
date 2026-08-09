namespace SmartRecruitment_Project.DTOs.Employers
{
    public class EmployerProfileDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string? Description { get; set; }

        public string? Website { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}