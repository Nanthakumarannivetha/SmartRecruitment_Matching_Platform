using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.Jobs
{
    public class JobVacancyDto
    {
        public int Id { get; set; }

        public int EmployerProfileId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int MinimumExperienceYears { get; set; }

        public EducationLevel RequiredEducationLevel { get; set; }

        public JobStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<string> RequiredSkills { get; set; }
            = new List<string>();
    }
}