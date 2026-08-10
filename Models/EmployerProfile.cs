namespace SmartRecruitment_Project.Models
{
    public class EmployerProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string? Description { get; set; }

        public string? Website { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; } = null!;

        public ICollection<JobVacancy> JobVacancies { get; set; }
            = new List<JobVacancy>();
    }
}