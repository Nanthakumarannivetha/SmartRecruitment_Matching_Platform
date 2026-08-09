using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.JobSeekers
{
    public class JobSeekerProfileDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int YearsOfExperience { get; set; }

        public EducationLevel EducationLevel { get; set; }

        public string? Summary { get; set; }

        public List<string> Skills { get; set; } = new();

        public CvDocumentDto? Cv { get; set; }
    }
}