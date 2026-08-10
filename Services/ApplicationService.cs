using Microsoft.EntityFrameworkCore;
using SmartRecruitment.API.DTOs.Applications;
using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment.API.Interfaces.Repositories;
using SmartRecruitment.API.Interfaces.Services;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMatchingService _matchingService;
    private readonly INotificationService _notificationService;
    private readonly IFileStorageService _fileStorageService;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        IMatchingService matchingService,
        INotificationService notificationService,
        IFileStorageService fileStorageService)
    {
        _applicationRepository = applicationRepository;
        _matchingService = matchingService;
        _notificationService = notificationService;
        _fileStorageService = fileStorageService;
    }

    public async Task<CreateApplicationResponseDto> ApplyAsync(
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

        var vacancy =
            await _applicationRepository.GetJobVacancyForMatchingAsync(
                jobId);

        if (vacancy == null)
        {
            throw new KeyNotFoundException(
                "Job vacancy was not found.");
        }

        if (vacancy.Status != JobStatus.Open)
        {
            throw new InvalidOperationException(
                "This vacancy is not open for applications.");
        }

        var alreadyApplied =
            await _applicationRepository.ApplicationExistsAsync(
                vacancy.Id,
                jobSeeker.Id);

        if (alreadyApplied)
        {
            throw new InvalidOperationException(
                "You have already applied for this vacancy.");
        }

        var matchResult =
            _matchingService.CalculateMatch(
                jobSeeker,
                vacancy);

        var application = new Application
        {
            JobVacancyId = vacancy.Id,
            JobSeekerProfileId = jobSeeker.Id,
            Status = ApplicationStatus.Applied,
            MatchScore = matchResult.TotalScore,
            AppliedAt = DateTime.UtcNow
        };

        try
        {
            await _applicationRepository.AddApplicationAsync(
                application);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException(
                "You have already applied for this vacancy.");
        }

        return new CreateApplicationResponseDto
        {
            ApplicationId = application.Id,
            JobVacancyId = application.JobVacancyId,
            Status = application.Status,
            MatchScore = application.MatchScore,
            AppliedAt = application.AppliedAt
        };
    }

    public async Task<List<MyApplicationDto>> GetMyApplicationsAsync(
        int userId)
    {
        var jobSeeker =
            await _applicationRepository.GetJobSeekerProfileByUserIdAsync(
                userId);

        if (jobSeeker == null)
        {
            throw new KeyNotFoundException(
                "Job seeker profile was not found.");
        }

        var applications =
            await _applicationRepository.GetApplicationsByJobSeekerAsync(
                jobSeeker.Id);

        return applications
            .Select(application => new MyApplicationDto
            {
                ApplicationId = application.Id,
                JobVacancyId = application.JobVacancyId,
                JobTitle = application.JobVacancy.Title,
                CompanyName =
                    application.JobVacancy
                        .EmployerProfile
                        .CompanyName,
                MatchScore = application.MatchScore,
                Status = application.Status,
                AppliedAt = application.AppliedAt,
                UpdatedAt = application.UpdatedAt
            })
            .ToList();
    }

    public async Task<List<ApplicantRankingDto>> GetRankedApplicantsAsync(
        int employerUserId,
        int jobId)
    {
        var vacancy =
            await _applicationRepository.GetJobVacancyWithEmployerAsync(
                jobId);

        if (vacancy == null)
        {
            throw new KeyNotFoundException(
                "Job vacancy was not found.");
        }

        if (vacancy.EmployerProfile.UserId != employerUserId)
        {
            throw new UnauthorizedAccessException(
                "You do not own this vacancy.");
        }

        var applications =
            await _applicationRepository.GetApplicationsByJobVacancyAsync(
                jobId);

        return applications
            .OrderByDescending(application =>
                application.MatchScore)
            .ThenBy(application =>
                application.AppliedAt)
            .Select(application => new ApplicantRankingDto
            {
                ApplicationId =
                    application.Id,

                JobSeekerProfileId =
                    application.JobSeekerProfileId,

                FullName =
                    application.JobSeekerProfile.FullName,

                Location =
                    application.JobSeekerProfile.Location,

                YearsOfExperience =
                    application.JobSeekerProfile
                        .YearsOfExperience,

                EducationLevel =
                    application.JobSeekerProfile
                        .EducationLevel,

                Skills =
                    application.JobSeekerProfile
                        .JobSeekerSkills
                        .Where(x => x.Skill != null)
                        .Select(x => x.Skill.Name)
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase)
                        .OrderBy(x => x)
                        .ToList(),

                MatchScore =
                    application.MatchScore,

                Status =
                    application.Status,

                AppliedAt =
                    application.AppliedAt
            })
            .ToList();
    }

    public async Task UpdateApplicationStatusAsync(
        int employerUserId,
        int applicationId,
        ApplicationStatus status)
    {
        if (!Enum.IsDefined(
                typeof(ApplicationStatus),
                status))
        {
            throw new InvalidOperationException(
                "Invalid application status.");
        }

        var application =
            await _applicationRepository
                .GetApplicationWithVacancyAsync(
                    applicationId);

        if (application == null)
        {
            throw new KeyNotFoundException(
                "Application was not found.");
        }

        if (application.JobVacancy
                .EmployerProfile
                .UserId != employerUserId)
        {
            throw new UnauthorizedAccessException(
                "You cannot update this application.");
        }

        application.Status = status;
        application.UpdatedAt = DateTime.UtcNow;

        // Save the application change first.
        await _applicationRepository.SaveChangesAsync();

        // Then create the in-app notification.
        await _notificationService.CreateNotificationAsync(
            application.JobSeekerProfile.UserId,
            NotificationType.ApplicationStatusChanged,
            "Application Status Updated",
            $"Your application status for " +
            $"{application.JobVacancy.Title} " +
            $"has been updated to {status}.");
    }

    public async Task<MatchResultDto> GetJobMatchAsync(
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

        var vacancy =
            await _applicationRepository.GetJobVacancyForMatchingAsync(
                jobId);

        if (vacancy == null)
        {
            throw new KeyNotFoundException(
                "Job vacancy was not found.");
        }

        return _matchingService.CalculateMatch(
            jobSeeker,
            vacancy);
    }

    public async Task<ApplicantCvFileDto> GetApplicantCvAsync(
        int employerUserId,
        int applicationId)
    {
        var application =
            await _applicationRepository
                .GetApplicationWithApplicantCvAsync(
                    applicationId);

        if (application == null)
        {
            throw new KeyNotFoundException(
                "Application was not found.");
        }

        if (application.JobVacancy
                .EmployerProfile
                .UserId != employerUserId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to access this applicant CV.");
        }

        var cv =
            application.JobSeekerProfile.CvDocument;

        if (cv == null)
        {
            throw new KeyNotFoundException(
                "Applicant CV was not found.");
        }

        await using var fileStream =
            await _fileStorageService.OpenFileAsync(
                cv.StoredFileName);

        using var memoryStream =
            new MemoryStream();

        await fileStream.CopyToAsync(
            memoryStream);

        return new ApplicantCvFileDto
        {
            FileBytes = memoryStream.ToArray(),
            ContentType = cv.ContentType,
            FileName = cv.OriginalFileName
        };
    }
}