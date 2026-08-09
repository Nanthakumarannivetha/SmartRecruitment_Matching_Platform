using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(int notificationId);

        Task<Notification> CreateAsync(Notification notification);

        Task<Notification> UpdateAsync(Notification notification);
    }
}