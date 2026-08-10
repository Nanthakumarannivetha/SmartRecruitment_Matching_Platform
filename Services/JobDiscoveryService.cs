using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment.API.Interfaces.Repositories;
using SmartRecruitment.API.Interfaces.Services;

namespace SmartRecruitment.API.Services;

public class JobDiscoveryService : IJobDiscoveryService
{
    private readonly IJobDiscoveryRepository _jobDiscoveryRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMatchingService _matchingService;

    public JobDiscoveryService(
        IJobDiscoveryRepository jobDiscoveryRepository,
        IApplicationRepository applicationRepository,
        IMatchingService matchingService)
    {
        _jobDiscoveryRepository = jobDiscoveryRepository;
        _applicationRepository = applicationRepository;
        _matchingService = matchingService;
    }

    public async Task<List<JobMatchDto>> GetOpenJobsAsync(
        int userId,
        JobSearchQueryDto query)
    {
        var jobSeeker =
            await _applicationRepository.GetJobSeekerProfileByUserIdAsync(
                userId);

        if (jobSeeker == null)
        {
            throw new KeyNotFoundException(
                "Job seeker profile was not found.");
        }

        var jobs =
            await _jobDiscoveryRepository.GetOpenJobsAsync(
                query.Search,
                query.Location);

        var result = new List<JobMatchDto>();

        foreach (var job in jobs)
        {
            var matchResult =
                _matchingService.CalculateMatch(
                    jobSeeker,
                    job);

            result.Add(new JobMatchDto
            {
                JobId = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,

                CompanyName =
                    job.EmployerProfile.CompanyName,

                MinimumExperienceYears =
                    job.MinimumExperienceYears,

                RequiredEducationLevel =
                    job.RequiredEducationLevel.ToString(),

                RequiredSkills =
                    job.JobVacancySkills
                        .Where(x => x.Skill != null)
                        .Select(x => x.Skill.Name)
                        .OrderBy(x => x)
                        .ToList(),

                MatchScore = matchResult.TotalScore,
                SkillsScore = matchResult.SkillsScore,
                ExperienceScore = matchResult.ExperienceScore,
                EducationScore = matchResult.EducationScore,
                LocationScore = matchResult.LocationScore,

                MatchedSkills = matchResult.MatchedSkills,
                MissingSkills = matchResult.MissingSkills
            });
        }

        return result
            .OrderByDescending(x => x.MatchScore)
            .ThenBy(x => x.Title)
            .ToList();
    }

    public async Task<JobMatchDto> GetJobByIdAsync(
        int userId,
        int jobId)
    {
        var jobSeeker =
            await _applicationRepository.GetJobSeekerProfileByUserIdAsync(
                userId);

        if (jobSeeker == null)
        {
            throw new KeyNotFoundException(
                "Job seeker profile was not found.");
        }

        var job =
            await _jobDiscoveryRepository.GetOpenJobByIdAsync(
                jobId);

        if (job == null)
        {
            throw new KeyNotFoundException(
                "Open job vacancy was not found.");
        }

        var matchResult =
            _matchingService.CalculateMatch(
                jobSeeker,
                job);

        return new JobMatchDto
        {
            JobId = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,

            CompanyName =
                job.EmployerProfile.CompanyName,

            MinimumExperienceYears =
                job.MinimumExperienceYears,

            RequiredEducationLevel =
                job.RequiredEducationLevel.ToString(),

            RequiredSkills =
                job.JobVacancySkills
                    .Where(x => x.Skill != null)
                    .Select(x => x.Skill.Name)
                    .OrderBy(x => x)
                    .ToList(),

            MatchScore = matchResult.TotalScore,
            SkillsScore = matchResult.SkillsScore,
            ExperienceScore = matchResult.ExperienceScore,
            EducationScore = matchResult.EducationScore,
            LocationScore = matchResult.LocationScore,

            MatchedSkills = matchResult.MatchedSkills,
            MissingSkills = matchResult.MissingSkills
        };
    }
}