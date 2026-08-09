using System.ComponentModel.DataAnnotations;

namespace SmartRecruitment_Project.DTOs.JobSeekers
{
    public class UpdateJobSeekerSkillsDto
    {
        [Required]
        public List<string> Skills { get; set; } = new();
    }
}