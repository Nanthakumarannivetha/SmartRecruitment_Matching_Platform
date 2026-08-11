using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.DTOs.Applications;

public class ApplicantRankingDto
{
    public int ApplicationId { get; set; }

    public int JobSeekerProfileId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Location { get; set; }

    public int YearsOfExperience { get; set; }

    public EducationLevel EducationLevel { get; set; }

    public List<string> Skills { get; set; } = new();

    public decimal MatchScore { get; set; }

    public ApplicationStatus Status { get; set; }

    public DateTime AppliedAt { get; set; }
}