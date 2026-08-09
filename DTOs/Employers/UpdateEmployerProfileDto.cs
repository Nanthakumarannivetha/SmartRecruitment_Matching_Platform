using System.ComponentModel.DataAnnotations;

namespace SmartRecruitment_Project.DTOs.Employers
{
    public class UpdateEmployerProfileDto
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Location { get; set; }

        [MaxLength(1500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Website { get; set; }
    }
}