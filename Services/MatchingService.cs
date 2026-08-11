using Microsoft.Extensions.Options;
using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment.API.Interfaces.Services;
using SmartRecruitment.API.Options;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment.API.Services;

public class MatchingService : IMatchingService
{
    private readonly MatchingOptions _options;

    public MatchingService(
        IOptions<MatchingOptions> options)
    {
        _options = options.Value;

        _options.Validate();
    }

    public MatchResultDto CalculateMatch(
        JobSeekerProfile seeker,
        JobVacancy vacancy)
    {
        // ---------------------------------------------
        // Candidate Skills
        // ---------------------------------------------
        var candidateSkills =
            seeker.JobSeekerSkills
                .Where(x => x.Skill != null)
                .Select(x => x.Skill.NormalizedName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

        // ---------------------------------------------
        // Required Vacancy Skills
        // ---------------------------------------------
        var requiredSkills =
            vacancy.JobVacancySkills
                .Where(x => x.Skill != null)
                .Select(x => x.Skill)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.NormalizedName))
                .GroupBy(
                    x => x.NormalizedName,
                    StringComparer.OrdinalIgnoreCase)
                .Select(x => x.First())
                .ToList();

        var matchedSkills =
            new List<string>();

        var missingSkills =
            new List<string>();

        foreach (var skill in requiredSkills)
        {
            if (candidateSkills.Contains(
                    skill.NormalizedName))
            {
                matchedSkills.Add(
                    skill.Name);
            }
            else
            {
                missingSkills.Add(
                    skill.Name);
            }
        }

        // ---------------------------------------------
        // Skills Score
        // ---------------------------------------------
        decimal skillsScore;

        if (requiredSkills.Count == 0)
        {
            skillsScore =
                _options.SkillsWeight;
        }
        else
        {
            skillsScore =
                ((decimal)matchedSkills.Count /
                 requiredSkills.Count)
                * _options.SkillsWeight;
        }

        // ---------------------------------------------
        // Experience Score
        // ---------------------------------------------
        decimal experienceScore;

        if (vacancy.MinimumExperienceYears <= 0)
        {
            experienceScore =
                _options.ExperienceWeight;
        }
        else if (
            seeker.YearsOfExperience >=
            vacancy.MinimumExperienceYears)
        {
            experienceScore =
                _options.ExperienceWeight;
        }
        else
        {
            var safeExperience =
                Math.Max(
                    seeker.YearsOfExperience,
                    0);

            experienceScore =
                ((decimal)safeExperience /
                 vacancy.MinimumExperienceYears)
                * _options.ExperienceWeight;
        }

        // ---------------------------------------------
        // Education Score
        // ---------------------------------------------
        decimal educationScore;

        if (seeker.EducationLevel >=
            vacancy.RequiredEducationLevel)
        {
            educationScore =
                _options.EducationWeight;
        }
        else
        {
            educationScore = 0m;
        }

        // ---------------------------------------------
        // Location Score
        // ---------------------------------------------
        var seekerLocation =
            seeker.Location?.Trim();

        var vacancyLocation =
            vacancy.Location?.Trim();

        decimal locationScore;

        if (!string.IsNullOrWhiteSpace(
                seekerLocation) &&
            !string.IsNullOrWhiteSpace(
                vacancyLocation) &&
            string.Equals(
                seekerLocation,
                vacancyLocation,
                StringComparison.OrdinalIgnoreCase))
        {
            locationScore =
                _options.LocationWeight;
        }
        else
        {
            locationScore = 0m;
        }

        // ---------------------------------------------
        // Final Score
        // ---------------------------------------------
        var totalScore =
            skillsScore +
            experienceScore +
            educationScore +
            locationScore;

        totalScore =
            Math.Clamp(
                totalScore,
                0m,
                _options.TotalWeight);

        return new MatchResultDto
        {
            TotalScore =
                Math.Round(
                    totalScore,
                    2),

            SkillsScore =
                Math.Round(
                    skillsScore,
                    2),

            ExperienceScore =
                Math.Round(
                    experienceScore,
                    2),

            EducationScore =
                Math.Round(
                    educationScore,
                    2),

            LocationScore =
                Math.Round(
                    locationScore,
                    2),

            MatchedSkills =
                matchedSkills
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList(),

            MissingSkills =
                missingSkills
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList()
        };
    }
}