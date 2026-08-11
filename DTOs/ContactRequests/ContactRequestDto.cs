using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.ContactRequests
{
    public class ContactRequestDto
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string? Message { get; set; }

        public ContactRequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RespondedAt { get; set; }
    }
}