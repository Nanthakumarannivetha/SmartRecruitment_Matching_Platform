using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public JobSeekerProfile? JobSeekerProfile { get; set; }

        public EmployerProfile? EmployerProfile { get; set; }

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}