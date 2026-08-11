using SmartRecruitment_Project.DTOs.JobSeekers;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;
using Microsoft.Extensions.Configuration;

namespace SmartRecruitment_Project.Services
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly IJobSeekerRepository _repository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _configuration;

        public JobSeekerService(
            IJobSeekerRepository repository,
            IFileStorageService fileStorageService,
            IConfiguration configuration)
        {
            _repository = repository;
            _fileStorageService = fileStorageService;
            _configuration = configuration;
        }

        public async Task<JobSeekerProfileDto?> GetProfileAsync(int userId)
        {
            var profile =
                await _repository.GetProfileByUserIdAsync(userId);

            if (profile == null)
            {
                return null;
            }

            return MapToDto(profile);
        }

        public async Task<JobSeekerProfileDto> UpdateProfileAsync(
            int userId,
            UpdateJobSeekerProfileDto dto)
        {
            if (!Enum.IsDefined(dto.EducationLevel))
            {
                throw new ArgumentException(
                    "Invalid education level.");
            }

            if (dto.YearsOfExperience < 0)
            {
                throw new ArgumentException(
                    "Years of experience cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Full name is required.");
            }

            var profile =
                await _repository.GetProfileByUserIdAsync(userId);

            if (profile == null)
            {
                profile = new JobSeekerProfile
                {
                    UserId = userId
                };

                await _repository.AddProfileAsync(profile);
            }

            profile.FullName = dto.FullName.Trim();
            profile.Location = dto.Location?.Trim();
            profile.YearsOfExperience = dto.YearsOfExperience;
            profile.EducationLevel = dto.EducationLevel;
            profile.Summary = dto.Summary?.Trim();
            profile.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            return MapToDto(profile);
        }

        private static JobSeekerProfileDto MapToDto(
            JobSeekerProfile profile)
        {
            return new JobSeekerProfileDto
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Location = profile.Location,
                YearsOfExperience = profile.YearsOfExperience,
                EducationLevel = profile.EducationLevel,
                Summary = profile.Summary,

                Skills = profile.JobSeekerSkills
                    .Select(x => x.Skill.Name)
                    .ToList(),

                Cv = profile.CvDocument == null
                    ? null
                    : new CvDocumentDto
                    {
                        OriginalFileName =
                            profile.CvDocument.OriginalFileName,
                        ContentType =
                            profile.CvDocument.ContentType,
                        FileSize =
                            profile.CvDocument.FileSize,
                        UploadedAt =
                            profile.CvDocument.UploadedAt
                    }
            };
        }

        public async Task<JobSeekerProfileDto> UpdateSkillsAsync(
            int userId,
            UpdateJobSeekerSkillsDto dto)
        {
            await using var transaction =
                await _repository.BeginTransactionAsync();

            try
            {
                var profile =
                    await _repository.GetProfileByUserIdAsync(userId);

                if (profile == null)
                {
                    throw new KeyNotFoundException(
                        "Job seeker profile not found.");
                }

                var skillNames = dto.Skills
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (skillNames.Any(x => x.Length > 100))
                {
                    throw new ArgumentException(
                        "Skill name cannot exceed 100 characters.");
                }

                profile.JobSeekerSkills.Clear();

                foreach (var skillName in skillNames)
                {
                    var normalizedName =
                        skillName.ToUpperInvariant();

                    var skill =
                        await _repository.GetSkillByNormalizedNameAsync(
                            normalizedName);

                    if (skill == null)
                    {
                        skill = new Skill
                        {
                            Name = skillName,
                            NormalizedName = normalizedName
                        };

                        await _repository.AddSkillAsync(skill);
                    }

                    profile.JobSeekerSkills.Add(
                        new JobSeekerSkill
                        {
                            JobSeekerProfile = profile,
                            Skill = skill
                        });
                }

                profile.UpdatedAt = DateTime.UtcNow;

                await _repository.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapToDto(profile);
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<CvDocumentDto> UploadCvAsync(
            int userId,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("CV file is empty.");
            }

            var maxFileSize =
                _configuration.GetValue<long>(
                    "CvStorage:MaxFileSizeBytes");

            if (maxFileSize <= 0)
            {
                throw new InvalidOperationException(
                    "CV maximum file size is not configured correctly.");
            }

            if (file.Length > maxFileSize)
            {
                throw new ArgumentException(
                    "CV file size must be 5 MB or less.");
            }

            var allowedExtensions = new[]
            {
                ".pdf",
                ".doc",
                ".docx"
            };

            var safeOriginalFileName =
                Path.GetFileName(file.FileName);

            var extension = Path.GetExtension(safeOriginalFileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    "Only PDF, DOC and DOCX files are allowed.");
            }

            var allowedContentTypes = new[]
            {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                throw new ArgumentException(
                    "Invalid CV content type.");
            }

            var hasValidSignature =
                await HasValidFileSignatureAsync(
                    file,
                    extension);

            if (!hasValidSignature)
            {
                throw new ArgumentException(
                    "CV file content does not match the selected file type.");
            }

            var profile =
                await _repository.GetProfileByUserIdAsync(userId);

            if (profile == null)
            {
                throw new KeyNotFoundException(
                    "Job seeker profile not found.");
            }

            var oldCv =
                await _repository.GetCvByProfileIdAsync(profile.Id);

            var oldStoredFileName = oldCv?.StoredFileName;

            var newStoredFileName =
                await _fileStorageService.SaveFileAsync(file);

            var uploadedAt = DateTime.UtcNow;

            try
            {
                if (oldCv != null)
                {
                    oldCv.OriginalFileName = safeOriginalFileName;
                    oldCv.StoredFileName = newStoredFileName;
                    oldCv.ContentType = file.ContentType;
                    oldCv.FileSize = file.Length;
                    oldCv.UploadedAt = uploadedAt;
                }
                else
                {
                    var cvDocument = new CvDocument
                    {
                        JobSeekerProfileId = profile.Id,
                        OriginalFileName = safeOriginalFileName,
                        StoredFileName = newStoredFileName,
                        ContentType = file.ContentType,
                        FileSize = file.Length,
                        UploadedAt = uploadedAt
                    };

                    await _repository.AddCvAsync(cvDocument);
                }

                await _repository.SaveChangesAsync();
            }
            catch
            {
                await _fileStorageService.DeleteFileAsync(
                    newStoredFileName);

                throw;
            }

            if (!string.IsNullOrWhiteSpace(oldStoredFileName))
            {
                await _fileStorageService.DeleteFileAsync(
                    oldStoredFileName);
            }

            return new CvDocumentDto
            {
                OriginalFileName = safeOriginalFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedAt = uploadedAt
            };
        }

        public async Task<(Stream Stream, string ContentType, string FileName)?>
            GetCvAsync(int userId)
        {
            var profile =
                await _repository.GetProfileByUserIdAsync(userId);

            if (profile == null || profile.CvDocument == null)
            {
                return null;
            }

            var cv = profile.CvDocument;

            var stream =
                await _fileStorageService.OpenFileAsync(
                    cv.StoredFileName);

            return (
                stream,
                cv.ContentType,
                cv.OriginalFileName
            );
        }

        private static async Task<bool> HasValidFileSignatureAsync(
            IFormFile file,
            string extension)
        {
            await using var stream = file.OpenReadStream();

            var header = new byte[8];

            var bytesRead =
                await stream.ReadAsync(
                    header.AsMemory(0, header.Length));

            if (bytesRead < 4)
            {
                return false;
            }

            if (extension == ".pdf")
            {
                return header[0] == 0x25 &&
                       header[1] == 0x50 &&
                       header[2] == 0x44 &&
                       header[3] == 0x46;
            }

            if (extension == ".doc")
            {
                return bytesRead >= 8 &&
                       header[0] == 0xD0 &&
                       header[1] == 0xCF &&
                       header[2] == 0x11 &&
                       header[3] == 0xE0 &&
                       header[4] == 0xA1 &&
                       header[5] == 0xB1 &&
                       header[6] == 0x1A &&
                       header[7] == 0xE1;
            }

            if (extension == ".docx")
            {
                return header[0] == 0x50 &&
                       header[1] == 0x4B &&
                       header[2] == 0x03 &&
                       header[3] == 0x04;
            }

            return false;
        }
    }
}
