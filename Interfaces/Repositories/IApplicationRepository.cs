using SmartRecruitment_Project.Models;

namespace SmartRecruitment.API.Interfaces.Repositories;

public interface IApplicationRepository
{
    Task<JobSeekerProfile?> GetJobSeekerProfileByUserIdAsync(int userId);

    Task<JobVacancy?> GetJobVacancyForMatchingAsync(int jobId);

    Task<bool> ApplicationExistsAsync(
        int jobVacancyId,
        int jobSeekerProfileId);

    Task<Application> AddApplicationAsync(
        Application application);

    Task<List<Application>> GetApplicationsByJobSeekerAsync(
        int jobSeekerProfileId);

    Task<JobVacancy?> GetJobVacancyWithEmployerAsync(
        int jobId);

    Task<List<Application>> GetApplicationsByJobVacancyAsync(
        int jobId);

    Task<Application?> GetApplicationWithVacancyAsync(
        int applicationId);

    Task<Application?> GetApplicationWithApplicantCvAsync(
        int applicationId);

    Task SaveChangesAsync();
}