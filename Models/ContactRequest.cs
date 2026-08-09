using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string? Message { get; set; }

        public ContactRequestStatus Status { get; set; }
            = ContactRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        // Navigation Property
        public Application Application { get; set; } = null!;
    }
}