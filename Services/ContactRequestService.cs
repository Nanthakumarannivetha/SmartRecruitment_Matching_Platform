using SmartRecruitment_Project.DTOs.ContactRequests;
using SmartRecruitment_Project.Exceptions;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Services
{
    public class ContactRequestService : IContactRequestService
    {
        private readonly IContactRequestRepository _contactRequestRepository;
        private readonly INotificationService _notificationService;

        public ContactRequestService(
            IContactRequestRepository contactRequestRepository,
            INotificationService notificationService)
        {
            _contactRequestRepository = contactRequestRepository;
            _notificationService = notificationService;
        }

        public async Task<ContactRequestDto> CreateAsync(
            int employerUserId,
            CreateContactRequestDto dto)
        {
            var employerProfile =
                await _contactRequestRepository
                    .GetEmployerProfileByUserIdAsync(employerUserId);

            if (employerProfile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            var application =
                await _contactRequestRepository
                    .GetApplicationWithDetailsAsync(dto.ApplicationId);

            if (application == null)
            {
                throw new NotFoundException(
                    "Application not found.");
            }

            if (application.JobVacancy.EmployerProfileId
                != employerProfile.Id)
            {
                throw new ForbiddenException(
                    "You cannot send a contact request for this application.");
            }

            var pendingExists =
                await _contactRequestRepository
                    .PendingContactRequestExistsAsync(dto.ApplicationId);

            if (pendingExists)
            {
                throw new ConflictException(
                    "A pending contact request already exists for this application.");
            }

            var contactRequest = new ContactRequest
            {
                ApplicationId = dto.ApplicationId,
                Message = dto.Message?.Trim(),
                Status = ContactRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _contactRequestRepository.CreateAsync(
                contactRequest);

            await _notificationService.CreateNotificationAsync(
                application.JobSeekerProfile.UserId,
                NotificationType.ContactRequestReceived,
                "Contact Request Received",
                "An employer has sent you a contact request.");

            return MapToDto(contactRequest);
        }

        public async Task<List<ContactRequestDto>> GetMineAsync(
            int jobSeekerUserId)
        {
            var jobSeekerProfile =
                await _contactRequestRepository
                    .GetJobSeekerProfileByUserIdAsync(jobSeekerUserId);

            if (jobSeekerProfile == null)
            {
                throw new NotFoundException(
                    "Job seeker profile not found.");
            }

            var requests =
                await _contactRequestRepository
                    .GetByJobSeekerProfileIdAsync(
                        jobSeekerProfile.Id);

            return requests
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ContactRequestDto> RespondAsync(
            int jobSeekerUserId,
            int contactRequestId,
            RespondContactRequestDto dto)
        {
            if (dto.Status != ContactRequestStatus.Accepted &&
                dto.Status != ContactRequestStatus.Declined)
            {
                throw new BadRequestException(
                    "Response must be Accepted or Declined.");
            }

            var jobSeekerProfile =
                await _contactRequestRepository
                    .GetJobSeekerProfileByUserIdAsync(jobSeekerUserId);

            if (jobSeekerProfile == null)
            {
                throw new NotFoundException(
                    "Job seeker profile not found.");
            }

            var contactRequest =
                await _contactRequestRepository
                    .GetByIdWithDetailsAsync(contactRequestId);

            if (contactRequest == null)
            {
                throw new NotFoundException(
                    "Contact request not found.");
            }

            if (contactRequest.Application.JobSeekerProfileId
                != jobSeekerProfile.Id)
            {
                throw new ForbiddenException(
                    "You cannot respond to this contact request.");
            }

            if (contactRequest.Status != ContactRequestStatus.Pending)
            {
                throw new ConflictException(
                    "This contact request has already been responded to.");
            }

            contactRequest.Status = dto.Status;
            contactRequest.RespondedAt = DateTime.UtcNow;

            await _contactRequestRepository.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                contactRequest.Application
                    .JobVacancy
                    .EmployerProfile
                    .UserId,
                NotificationType.ContactRequestResponded,
                "Contact Request Responded",
                $"The job seeker has {dto.Status.ToString().ToLower()} your contact request.");

            return MapToDto(contactRequest);
        }

        private static ContactRequestDto MapToDto(
            ContactRequest contactRequest)
        {
            return new ContactRequestDto
            {
                Id = contactRequest.Id,
                ApplicationId = contactRequest.ApplicationId,
                Message = contactRequest.Message,
                Status = contactRequest.Status,
                CreatedAt = contactRequest.CreatedAt,
                RespondedAt = contactRequest.RespondedAt
            };
        }
    }
}