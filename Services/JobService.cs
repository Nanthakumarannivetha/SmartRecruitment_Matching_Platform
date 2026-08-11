using SmartRecruitment_Project.DTOs.Jobs;
using SmartRecruitment_Project.Exceptions;
using SmartRecruitment_Project.Interfaces.Repositories;
using SmartRecruitment_Project.Interfaces.Services;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IEmployerRepository _employerRepository;

        public JobService(
            IJobRepository jobRepository,
            IEmployerRepository employerRepository)
        {
            _jobRepository = jobRepository;
            _employerRepository = employerRepository;
        }

        public async Task<JobVacancyDto> CreateJobAsync(
            int userId,
            CreateJobVacancyDto dto)
        {
            var employerProfile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (employerProfile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            var jobVacancy = new JobVacancy
            {
                EmployerProfileId = employerProfile.Id,
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                Location = dto.Location?.Trim(),
                MinimumExperienceYears =
                    dto.MinimumExperienceYears,
                RequiredEducationLevel =
                    dto.RequiredEducationLevel,
                Status = JobStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            jobVacancy =
                await _jobRepository.CreateAsync(jobVacancy);

            await SaveRequiredSkillsAsync(
                jobVacancy.Id,
                dto.RequiredSkills);

            var savedJob =
                await _jobRepository.GetByIdAsync(jobVacancy.Id);

            return MapToDto(savedJob!);
        }

        public async Task<List<JobVacancyDto>> GetMyJobsAsync(
            int userId)
        {
            var employerProfile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (employerProfile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            var jobs =
                await _jobRepository
                    .GetByEmployerProfileIdAsync(
                        employerProfile.Id);

            return jobs
                .Select(MapToDto)
                .ToList();
        }

        public async Task<JobVacancyDto> GetJobByIdAsync(
            int userId,
            int jobId)
        {
            var employerProfile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (employerProfile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            var job =
                await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                throw new NotFoundException(
                    "Job vacancy not found.");
            }

            if (job.EmployerProfileId != employerProfile.Id)
            {
                throw new ForbiddenException(
                    "You cannot access this job vacancy.");
            }

            return MapToDto(job);
        }

        public async Task<JobVacancyDto> UpdateJobAsync(
            int userId,
            int jobId,
            UpdateJobVacancyDto dto)
        {
            var employerProfile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (employerProfile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            var job =
                await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                throw new NotFoundException(
                    "Job vacancy not found.");
            }

            if (job.EmployerProfileId != employerProfile.Id)
            {
                throw new ForbiddenException(
                    "You cannot update this job vacancy.");
            }

            await using var transaction =
                await _jobRepository.BeginTransactionAsync();

            try
            {
                job.Title = dto.Title.Trim();
                job.Description = dto.Description.Trim();
                job.Location = dto.Location?.Trim();
                job.MinimumExperienceYears =
                    dto.MinimumExperienceYears;
                job.RequiredEducationLevel =
                    dto.RequiredEducationLevel;
                job.UpdatedAt = DateTime.UtcNow;

                await _jobRepository.UpdateAsync(job);

                await _jobRepository
                    .RemoveVacancySkillsAsync(job.Id);

                await SaveRequiredSkillsAsync(
                    job.Id,
                    dto.RequiredSkills);

                await transaction.CommitAsync();

                var updatedJob =
                    await _jobRepository.GetByIdAsync(job.Id);

                return MapToDto(updatedJob!);
            }
            catch
            {
                await _jobRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<JobVacancyDto> CloseJobAsync(
            int userId,
            int jobId)
        {
            var employerProfile =
                await _employerRepository.GetByUserIdAsync(userId);

            if (employerProfile == null)
            {
                throw new NotFoundException(
                    "Employer profile not found.");
            }

            var job =
                await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                throw new NotFoundException(
                    "Job vacancy not found.");
            }

            if (job.EmployerProfileId != employerProfile.Id)
            {
                throw new ForbiddenException(
                    "You cannot close this job vacancy.");
            }

            job.Status = JobStatus.Closed;
            job.UpdatedAt = DateTime.UtcNow;

            await _jobRepository.UpdateAsync(job);

            return MapToDto(job);
        }

        private async Task SaveRequiredSkillsAsync(
            int jobVacancyId,
            List<string> requiredSkills)
        {
            var cleanSkills = requiredSkills
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var jobVacancySkills =
                new List<JobVacancySkill>();

            foreach (var skillName in cleanSkills)
            {
                var normalizedName =
                    skillName.ToUpperInvariant();

                var skill =
                    await _jobRepository
                        .GetSkillByNormalizedNameAsync(
                            normalizedName);

                if (skill == null)
                {
                    skill = new Skill
                    {
                        Name = skillName,
                        NormalizedName = normalizedName
                    };

                    skill =
                        await _jobRepository
                            .CreateSkillAsync(skill);
                }

                jobVacancySkills.Add(
                    new JobVacancySkill
                    {
                        JobVacancyId = jobVacancyId,
                        SkillId = skill.Id
                    });
            }

            await _jobRepository
                .AddVacancySkillsAsync(jobVacancySkills);
        }

        private static JobVacancyDto MapToDto(
            JobVacancy job)
        {
            return new JobVacancyDto
            {
                Id = job.Id,
                EmployerProfileId =
                    job.EmployerProfileId,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                MinimumExperienceYears =
                    job.MinimumExperienceYears,
                RequiredEducationLevel =
                    job.RequiredEducationLevel,
                Status = job.Status,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                RequiredSkills =
                    job.JobVacancySkills
                        .Select(x => x.Skill.Name)
                        .ToList()
            };
        }
    }
}