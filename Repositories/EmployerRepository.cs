using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Repositories
{
    public class EmployerRepository : IEmployerRepository
    {
        private readonly AppDbContext _context;

        public EmployerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployerProfile?> GetByUserIdAsync(int userId)
        {
            return await _context.EmployerProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<EmployerProfile> CreateAsync(
            EmployerProfile employerProfile)
        {
            _context.EmployerProfiles.Add(employerProfile);

            await _context.SaveChangesAsync();

            return employerProfile;
        }

        public async Task<EmployerProfile> UpdateAsync(
            EmployerProfile employerProfile)
        {
            _context.EmployerProfiles.Update(employerProfile);

            await _context.SaveChangesAsync();

            return employerProfile;
        }
    }
}