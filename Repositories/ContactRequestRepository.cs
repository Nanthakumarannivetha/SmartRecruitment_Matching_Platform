using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Repositories
{
    public class ContactRequestRepository : IContactRequestRepository
    {
        private readonly AppDbContext _context;

        public ContactRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployerProfile?> GetEmployerProfileByUserIdAsync(
            int userId)
        {
            return await _context.EmployerProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<JobSeekerProfile?> GetJobSeekerProfileByUserIdAsync(
            int userId)
        {
            return await _context.JobSeekerProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Application?> GetApplicationWithDetailsAsync(
            int applicationId)
        {
            return await _context.Applications
                .Include(x => x.JobVacancy)
                    .ThenInclude(x => x.EmployerProfile)
                .Include(x => x.JobSeekerProfile)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == applicationId);
        }

        public async Task<bool> PendingContactRequestExistsAsync(
            int applicationId)
        {
            return await _context.ContactRequests
                .AnyAsync(x =>
                    x.ApplicationId == applicationId &&
                    x.Status == ContactRequestStatus.Pending);
        }

        public async Task<ContactRequest> CreateAsync(
            ContactRequest contactRequest)
        {
            await _context.ContactRequests.AddAsync(contactRequest);

            await _context.SaveChangesAsync();

            return contactRequest;
        }

        public async Task<List<ContactRequest>> GetByJobSeekerProfileIdAsync(
            int jobSeekerProfileId)
        {
            return await _context.ContactRequests
                .AsNoTracking()
                .Include(x => x.Application)
                    .ThenInclude(x => x.JobVacancy)
                        .ThenInclude(x => x.EmployerProfile)
                .Where(x =>
                    x.Application.JobSeekerProfileId == jobSeekerProfileId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContactRequest?> GetByIdWithDetailsAsync(
            int contactRequestId)
        {
            return await _context.ContactRequests
                .Include(x => x.Application)
                    .ThenInclude(x => x.JobVacancy)
                        .ThenInclude(x => x.EmployerProfile)
                            .ThenInclude(x => x.User)
                .Include(x => x.Application)
                    .ThenInclude(x => x.JobSeekerProfile)
                        .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == contactRequestId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}