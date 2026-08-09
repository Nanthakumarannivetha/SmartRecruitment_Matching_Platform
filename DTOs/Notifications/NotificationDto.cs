using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}