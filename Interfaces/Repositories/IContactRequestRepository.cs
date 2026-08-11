using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Interfaces.Repositories
{
    public interface IContactRequestRepository
    {
        Task<EmployerProfile?> GetEmployerProfileByUserIdAsync(int userId);

        Task<JobSeekerProfile?> GetJobSeekerProfileByUserIdAsync(int userId);

        Task<Application?> GetApplicationWithDetailsAsync(int applicationId);

        Task<bool> PendingContactRequestExistsAsync(int applicationId);

        Task<ContactRequest> CreateAsync(ContactRequest contactRequest);

        Task<List<ContactRequest>> GetByJobSeekerProfileIdAsync(
            int jobSeekerProfileId);

        Task<ContactRequest?> GetByIdWithDetailsAsync(int contactRequestId);

        Task SaveChangesAsync();
    }
}