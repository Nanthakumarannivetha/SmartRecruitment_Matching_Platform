using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobVacancy> CreateAsync(
            JobVacancy jobVacancy)
        {
            _context.JobVacancies.Add(jobVacancy);

            await _context.SaveChangesAsync();

            return jobVacancy;
        }

        public async Task<List<JobVacancy>> GetByEmployerProfileIdAsync(
            int employerProfileId)
        {
            return await _context.JobVacancies
                .Include(x => x.JobVacancySkills)
                    .ThenInclude(x => x.Skill)
                .Where(x =>
                    x.EmployerProfileId == employerProfileId)
                .ToListAsync();
        }

        public async Task<JobVacancy?> GetByIdAsync(
            int jobId)
        {
            return await _context.JobVacancies
                .Include(x => x.JobVacancySkills)
                    .ThenInclude(x => x.Skill)
                .FirstOrDefaultAsync(x => x.Id == jobId);
        }

        public async Task<JobVacancy> UpdateAsync(
            JobVacancy jobVacancy)
        {
            _context.JobVacancies.Update(jobVacancy);

            await _context.SaveChangesAsync();

            return jobVacancy;
        }

        public async Task<Skill?> GetSkillByNormalizedNameAsync(
            string normalizedName)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(x =>
                    x.NormalizedName == normalizedName);
        }

        public async Task<Skill> CreateSkillAsync(
            Skill skill)
        {
            _context.Skills.Add(skill);

            await _context.SaveChangesAsync();

            return skill;
        }

        public async Task RemoveVacancySkillsAsync(
            int jobVacancyId)
        {
            var vacancySkills =
                await _context.JobVacancySkills
                    .Where(x =>
                        x.JobVacancyId == jobVacancyId)
                    .ToListAsync();

            _context.JobVacancySkills
                .RemoveRange(vacancySkills);

            await _context.SaveChangesAsync();
        }

        public async Task AddVacancySkillsAsync(
            List<JobVacancySkill> jobVacancySkills)
        {
            if (jobVacancySkills.Count == 0)
            {
                return;
            }

            _context.JobVacancySkills
                .AddRange(jobVacancySkills);

            await _context.SaveChangesAsync();
        }

        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _context.Database.RollbackTransactionAsync();
            }
        }
    }
}
