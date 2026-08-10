namespace SmartRecruitment.API.Options;

public class MatchingOptions
{
    public const string SectionName = "Matching";

    // Total = 100
    public decimal SkillsWeight { get; set; } = 60m;

    public decimal ExperienceWeight { get; set; } = 20m;

    public decimal EducationWeight { get; set; } = 10m;

    public decimal LocationWeight { get; set; } = 10m;

    public decimal TotalWeight =>
        SkillsWeight +
        ExperienceWeight +
        EducationWeight +
        LocationWeight;

    public void Validate()
    {
        if (SkillsWeight < 0 ||
            ExperienceWeight < 0 ||
            EducationWeight < 0 ||
            LocationWeight < 0)
        {
            throw new InvalidOperationException(
                "Matching weights cannot be negative.");
        }

        if (TotalWeight != 100m)
        {
            throw new InvalidOperationException(
                "Matching weights must total exactly 100.");
        }
    }
}