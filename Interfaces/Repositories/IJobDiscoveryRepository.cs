using SmartRecruitment_Project.Models;

namespace SmartRecruitment.API.Interfaces.Repositories;

public interface IJobDiscoveryRepository
{
    Task<List<JobVacancy>> GetOpenJobsAsync(
        string? search,
        string? location);

    Task<JobVacancy?> GetOpenJobByIdAsync(int jobId);
}