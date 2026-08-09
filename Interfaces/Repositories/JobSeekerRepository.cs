using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace SmartRecruitment_Project.Repositories
{
    public class JobSeekerRepository : IJobSeekerRepository
    {
        private readonly AppDbContext _context;

        public JobSeekerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobSeekerProfile?> GetProfileByUserIdAsync(int userId)
        {
            return await _context.JobSeekerProfiles
                .Include(x => x.JobSeekerSkills)
                    .ThenInclude(x => x.Skill)
                .Include(x => x.CvDocument)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task AddProfileAsync(JobSeekerProfile profile)
        {
            await _context.JobSeekerProfiles.AddAsync(profile);
        }

        public async Task<Skill?> GetSkillByNormalizedNameAsync(
            string normalizedName)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(x =>
                    x.NormalizedName == normalizedName);
        }

        public async Task AddSkillAsync(Skill skill)
        {
            await _context.Skills.AddAsync(skill);
        }

        public async Task<CvDocument?> GetCvByProfileIdAsync(
            int jobSeekerProfileId)
        {
            return await _context.CvDocuments
                .FirstOrDefaultAsync(x =>
                    x.JobSeekerProfileId == jobSeekerProfileId);
        }

        public async Task AddCvAsync(CvDocument cvDocument)
        {
            await _context.CvDocuments.AddAsync(cvDocument);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
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