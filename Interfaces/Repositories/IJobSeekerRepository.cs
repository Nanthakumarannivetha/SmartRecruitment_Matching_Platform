using SmartRecruitment_Project.Models;
using Microsoft.EntityFrameworkCore.Storage;
namespace SmartRecruitment_Project.Interfaces.Repositories
{
    public interface IJobSeekerRepository
    {
        Task<JobSeekerProfile?> GetProfileByUserIdAsync(int userId);

        Task AddProfileAsync(JobSeekerProfile profile);

        Task<Skill?> GetSkillByNormalizedNameAsync(string normalizedName);

        Task AddSkillAsync(Skill skill);

        Task<CvDocument?> GetCvByProfileIdAsync(int jobSeekerProfileId);

        Task AddCvAsync(CvDocument cvDocument);

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task RollbackTransactionAsync();
        Task SaveChangesAsync();
    }
}