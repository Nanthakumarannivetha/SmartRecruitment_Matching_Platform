using SmartRecruitment_Project.DTOs.Jobs;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IJobService
    {
        Task<JobVacancyDto> CreateJobAsync(
            int userId,
            CreateJobVacancyDto dto);

        Task<List<JobVacancyDto>> GetMyJobsAsync(
            int userId);

        Task<JobVacancyDto> GetJobByIdAsync(
            int userId,
            int jobId);

        Task<JobVacancyDto> UpdateJobAsync(
            int userId,
            int jobId,
            UpdateJobVacancyDto dto);

        Task<JobVacancyDto> CloseJobAsync(
            int userId,
            int jobId);
    }
}