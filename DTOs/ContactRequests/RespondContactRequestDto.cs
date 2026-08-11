using System.ComponentModel.DataAnnotations;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.DTOs.ContactRequests
{
    public class RespondContactRequestDto
    {
        [Required]
        public ContactRequestStatus Status { get; set; }
    }
}