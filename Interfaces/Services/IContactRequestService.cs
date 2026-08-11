using SmartRecruitment_Project.DTOs.ContactRequests;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IContactRequestService
    {
        Task<ContactRequestDto> CreateAsync(
            int employerUserId,
            CreateContactRequestDto dto);

        Task<List<ContactRequestDto>> GetMineAsync(
            int jobSeekerUserId);

        Task<ContactRequestDto> RespondAsync(
            int jobSeekerUserId,
            int contactRequestId,
            RespondContactRequestDto dto);
    }
}