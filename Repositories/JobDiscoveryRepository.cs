using Microsoft.EntityFrameworkCore;
using SmartRecruitment.API.Interfaces.Repositories;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.Repositories;

public class JobDiscoveryRepository : IJobDiscoveryRepository
{
    private readonly AppDbContext _context;

    public JobDiscoveryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobVacancy>> GetOpenJobsAsync(
        string? search,
        string? location)
    {
        var query = _context.JobVacancies
            .AsNoTracking()
            .Include(x => x.EmployerProfile)
            .Include(x => x.JobVacancySkills)
                .ThenInclude(x => x.Skill)
            .Where(x => x.Status == JobStatus.Open)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchValue = search.Trim();

            query = query.Where(x =>
                x.Title.Contains(searchValue) ||
                x.Description.Contains(searchValue));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            var locationValue = location.Trim();

            query = query.Where(x =>
                x.Location != null &&
                x.Location.Contains(locationValue));
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<JobVacancy?> GetOpenJobByIdAsync(int jobId)
    {
        return await _context.JobVacancies
            .AsNoTracking()
            .Include(x => x.EmployerProfile)
            .Include(x => x.JobVacancySkills)
                .ThenInclude(x => x.Skill)
            .FirstOrDefaultAsync(x =>
                x.Id == jobId &&
                x.Status == JobStatus.Open);
    }
}