namespace SmartRecruitment.API.DTOs.Jobs;

public class MatchResultDto
{
    public decimal TotalScore { get; set; }

    public decimal SkillsScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }

    public List<string> MatchedSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();
}