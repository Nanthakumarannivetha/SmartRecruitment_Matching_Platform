using SmartRecruitment_Project.DTOs.Notifications;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync(
            int userId);

        Task<NotificationDto> MarkAsReadAsync(
            int userId,
            int notificationId);

        Task CreateNotificationAsync(
            int userId,
            NotificationType type,
            string title,
            string message);
    }
}