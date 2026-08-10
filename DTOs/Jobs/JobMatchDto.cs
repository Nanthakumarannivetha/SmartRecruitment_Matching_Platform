namespace SmartRecruitment.API.DTOs.Jobs;

public class JobMatchDto
{
    public int JobId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Location { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public int MinimumExperienceYears { get; set; }

    public string RequiredEducationLevel { get; set; } = string.Empty;

    public List<string> RequiredSkills { get; set; } = new();

    public decimal MatchScore { get; set; }

    public decimal SkillsScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }

    public List<string> MatchedSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();
}