using System.ComponentModel.DataAnnotations;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.Jobs
{
    public class UpdateJobVacancyDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Location { get; set; }

        [Range(0, 50)]
        public int MinimumExperienceYears { get; set; }

        [Required]
        public EducationLevel RequiredEducationLevel { get; set; }

        public List<string> RequiredSkills { get; set; }
            = new List<string>();
    }
}