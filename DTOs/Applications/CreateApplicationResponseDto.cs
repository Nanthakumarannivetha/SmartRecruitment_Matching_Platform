using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.DTOs.Applications;

public class CreateApplicationResponseDto
{
    public int ApplicationId { get; set; }

    public int JobVacancyId { get; set; }

    public ApplicationStatus Status { get; set; }

    public decimal MatchScore { get; set; }

    public DateTime AppliedAt { get; set; }
}