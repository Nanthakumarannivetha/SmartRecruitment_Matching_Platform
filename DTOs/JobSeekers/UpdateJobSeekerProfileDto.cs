using System.ComponentModel.DataAnnotations;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.JobSeekers
{
    public class UpdateJobSeekerProfileDto
    {
        [Required]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "FullName cannot be blank.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Location { get; set; }

        [Range(0, 50)]
        public int YearsOfExperience { get; set; }

        [Required]
        public EducationLevel EducationLevel { get; set; }

        [StringLength(1000)]
        public string? Summary { get; set; }
    }
}