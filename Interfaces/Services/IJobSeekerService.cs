using SmartRecruitment_Project.DTOs.JobSeekers;

namespace SmartRecruitment_Project.Interfaces.Services
{
    public interface IJobSeekerService
    {
        Task<JobSeekerProfileDto?> GetProfileAsync(int userId);

        Task<JobSeekerProfileDto> UpdateProfileAsync(
            int userId,
            UpdateJobSeekerProfileDto dto);

        Task<JobSeekerProfileDto> UpdateSkillsAsync(
            int userId,
            UpdateJobSeekerSkillsDto dto);

        Task<CvDocumentDto> UploadCvAsync(
            int userId,
            IFormFile file);

        Task<(Stream Stream, string ContentType, string FileName)?>
            GetCvAsync(int userId);
    }
}