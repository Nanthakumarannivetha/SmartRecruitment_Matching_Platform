using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Data;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetByUserIdAsync(
            int userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(
            int notificationId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == notificationId);
        }

        public async Task<Notification> CreateAsync(
            Notification notification)
        {
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<Notification> UpdateAsync(
            Notification notification)
        {
            _context.Notifications.Update(notification);

            await _context.SaveChangesAsync();

            return notification;
        }
    }
}