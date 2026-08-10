using System.ComponentModel.DataAnnotations;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    [Required]
    [EnumDataType(typeof(ApplicationStatus))]
    public ApplicationStatus Status { get; set; }
}