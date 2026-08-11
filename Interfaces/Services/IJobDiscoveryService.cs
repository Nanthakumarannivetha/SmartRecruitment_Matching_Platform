using SmartRecruitment.API.DTOs.Jobs;

namespace SmartRecruitment.API.Interfaces.Services;

public interface IJobDiscoveryService
{
    Task<List<JobMatchDto>> GetOpenJobsAsync(
        int userId,
        JobSearchQueryDto query);

    Task<JobMatchDto> GetJobByIdAsync(
        int userId,
        int jobId);
}