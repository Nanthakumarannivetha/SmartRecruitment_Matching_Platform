using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Interfaces.Repositories
{
    public interface IJobRepository
    {
        Task<JobVacancy> CreateAsync(
            JobVacancy jobVacancy);

        Task<List<JobVacancy>> GetByEmployerProfileIdAsync(
            int employerProfileId);

        Task<JobVacancy?> GetByIdAsync(
            int jobId);

        Task<JobVacancy> UpdateAsync(
            JobVacancy jobVacancy);

        Task<Skill?> GetSkillByNormalizedNameAsync(
            string normalizedName);

        Task<Skill> CreateSkillAsync(
            Skill skill);

        Task RemoveVacancySkillsAsync(
            int jobVacancyId);

        Task AddVacancySkillsAsync(
            List<JobVacancySkill> jobVacancySkills);

        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync();

        Task RollbackTransactionAsync();
    }
}