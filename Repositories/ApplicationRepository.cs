using Microsoft.EntityFrameworkCore;
using SmartRecruitment.API.Interfaces.Repositories;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment.API.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly AppDbContext _context;

    public ApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<JobSeekerProfile?> GetJobSeekerProfileByUserIdAsync(
        int userId)
    {
        return await _context.JobSeekerProfiles
            .Include(x => x.JobSeekerSkills)
                .ThenInclude(x => x.Skill)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<JobVacancy?> GetJobVacancyForMatchingAsync(
        int jobId)
    {
        return await _context.JobVacancies
            .Include(x => x.JobVacancySkills)
                .ThenInclude(x => x.Skill)
            .FirstOrDefaultAsync(x => x.Id == jobId);
    }

    public async Task<bool> ApplicationExistsAsync(
        int jobVacancyId,
        int jobSeekerProfileId)
    {
        return await _context.Applications
            .AnyAsync(x =>
                x.JobVacancyId == jobVacancyId &&
                x.JobSeekerProfileId == jobSeekerProfileId);
    }

    public async Task<Application> AddApplicationAsync(
        Application application)
    {
        await _context.Applications.AddAsync(application);

        await _context.SaveChangesAsync();

        return application;
    }

    public async Task<List<Application>> GetApplicationsByJobSeekerAsync(
        int jobSeekerProfileId)
    {
        return await _context.Applications
            .AsNoTracking()
            .Include(x => x.JobVacancy)
                .ThenInclude(x => x.EmployerProfile)
            .Where(x =>
                x.JobSeekerProfileId == jobSeekerProfileId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();
    }

    public async Task<JobVacancy?> GetJobVacancyWithEmployerAsync(
        int jobId)
    {
        return await _context.JobVacancies
            .Include(x => x.EmployerProfile)
            .FirstOrDefaultAsync(x => x.Id == jobId);
    }

    public async Task<List<Application>> GetApplicationsByJobVacancyAsync(
        int jobId)
    {
        return await _context.Applications
            .AsNoTracking()
            .Include(x => x.JobSeekerProfile)
                .ThenInclude(x => x.JobSeekerSkills)
                    .ThenInclude(x => x.Skill)
            .Where(x => x.JobVacancyId == jobId)
            .OrderByDescending(x => x.MatchScore)
            .ThenBy(x => x.AppliedAt)
            .ToListAsync();
    }

    public async Task<Application?> GetApplicationWithVacancyAsync(
        int applicationId)
    {
        return await _context.Applications
            .Include(x => x.JobVacancy)
                .ThenInclude(x => x.EmployerProfile)
            .Include(x => x.JobSeekerProfile)
            .FirstOrDefaultAsync(x =>
                x.Id == applicationId);
    }

    public async Task<Application?> GetApplicationWithApplicantCvAsync(
        int applicationId)
    {
        return await _context.Applications
            .AsNoTracking()
            .Include(x => x.JobVacancy)
                .ThenInclude(x => x.EmployerProfile)
            .Include(x => x.JobSeekerProfile)
                .ThenInclude(x => x.CvDocument)
            .FirstOrDefaultAsync(x =>
                x.Id == applicationId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}