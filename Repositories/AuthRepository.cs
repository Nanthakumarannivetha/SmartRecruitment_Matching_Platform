using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}