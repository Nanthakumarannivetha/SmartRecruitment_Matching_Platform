using SmartRecruitment_Project.DTOs.Notifications;
using SmartRecruitment_Project.Exceptions;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(
            INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> GetMyNotificationsAsync(
            int userId)
        {
            var notifications =
                await _notificationRepository.GetByUserIdAsync(userId);

            return notifications
                .Select(MapToDto)
                .ToList();
        }

        public async Task<NotificationDto> MarkAsReadAsync(
            int userId,
            int notificationId)
        {
            var notification =
                await _notificationRepository.GetByIdAsync(
                    notificationId);

            if (notification == null)
            {
                throw new NotFoundException(
                    "Notification not found.");
            }

            if (notification.UserId != userId)
            {
                throw new ForbiddenException(
                    "You cannot access this notification.");
            }

            notification.IsRead = true;

            await _notificationRepository.UpdateAsync(
                notification);

            return MapToDto(notification);
        }

        public async Task CreateNotificationAsync(
            int userId,
            NotificationType type,
            string title,
            string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(
                notification);
        }

        private static NotificationDto MapToDto(
            Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}