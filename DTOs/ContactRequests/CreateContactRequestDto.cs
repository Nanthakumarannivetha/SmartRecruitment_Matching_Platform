using System.ComponentModel.DataAnnotations;

namespace SmartRecruitment_Project.DTOs.ContactRequests
{
    public class CreateContactRequestDto
    {
        [Required]
        public int ApplicationId { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }
    }
}