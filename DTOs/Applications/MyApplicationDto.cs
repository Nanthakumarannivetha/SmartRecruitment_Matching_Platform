using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.DTOs.Applications;

public class MyApplicationDto
{
    public int ApplicationId { get; set; }

    public int JobVacancyId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public decimal MatchScore { get; set; }

    public ApplicationStatus Status { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}