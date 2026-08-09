using SmartRecruitment_Project.Models.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace SmartRecruitment_Project.Models
{
    public class JobVacancy
    {
        public int Id { get; set; }

        public int EmployerProfileId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int MinimumExperienceYears { get; set; }

        public EducationLevel RequiredEducationLevel { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public EmployerProfile EmployerProfile { get; set; } = null!;

        public ICollection<JobVacancySkill> JobVacancySkills { get; set; }
            = new List<JobVacancySkill>();

        public ICollection<Application> Applications { get; set; }
            = new List<Application>();
    }
}