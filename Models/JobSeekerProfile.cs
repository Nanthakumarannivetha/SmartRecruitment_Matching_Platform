using SmartRecruitment_Project.Models.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace SmartRecruitment_Project.Models
{
    public class JobSeekerProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int YearsOfExperience { get; set; }

        public EducationLevel EducationLevel { get; set; }

        public string? Summary { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User User { get; set; } = null!;

        public CvDocument? CvDocument { get; set; }

        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
            = new List<JobSeekerSkill>();

        public ICollection<Application> Applications { get; set; }
            = new List<Application>();
    }
}
