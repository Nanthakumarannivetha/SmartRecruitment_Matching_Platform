using SmartRecruitment.API.DTOs.Applications;
using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.Interfaces.Services;

public interface IApplicationService
{
    Task<CreateApplicationResponseDto> ApplyAsync(
        int userId,
        int jobId);

    Task<List<MyApplicationDto>> GetMyApplicationsAsync(
        int userId);

    Task<List<ApplicantRankingDto>> GetRankedApplicantsAsync(
        int employerUserId,
        int jobId);

    Task UpdateApplicationStatusAsync(
        int employerUserId,
        int applicationId,
        ApplicationStatus status);

    Task<MatchResultDto> GetJobMatchAsync(
        int userId,
        int jobId);

    Task<ApplicantCvFileDto> GetApplicantCvAsync(
        int employerUserId,
        int applicationId);
}