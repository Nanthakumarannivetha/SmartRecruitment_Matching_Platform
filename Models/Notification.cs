using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public User User { get; set; } = null!;
    }
}