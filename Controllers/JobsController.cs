using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.DTOs.Jobs;
using SmartRecruitment_Project.Helpers;
using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employer")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        // Create new vacancy
        [HttpPost]
        public async Task<IActionResult> CreateJob(
            CreateJobVacancyDto dto)
        {
            var userId = User.GetUserId();

            var result =
                await _jobService.CreateJobAsync(
                    userId,
                    dto);

            return Ok(result);
        }

        // Get logged-in employer's vacancies
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyJobs()
        {
            var userId = User.GetUserId();

            var result =
                await _jobService.GetMyJobsAsync(userId);

            return Ok(result);
        }

        // Get one owned vacancy
        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetJob(
            int jobId)
        {
            var userId = User.GetUserId();

            var result =
                await _jobService.GetJobByIdAsync(
                    userId,
                    jobId);

            return Ok(result);
        }

        // Update vacancy
        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJob(
            int jobId,
            UpdateJobVacancyDto dto)
        {
            var userId = User.GetUserId();

            var result =
                await _jobService.UpdateJobAsync(
                    userId,
                    jobId,
                    dto);

            return Ok(result);
        }

        // Close vacancy
        [HttpPatch("{jobId}/close")]
        public async Task<IActionResult> CloseJob(
            int jobId)
        {
            var userId = User.GetUserId();

            var result =
                await _jobService.CloseJobAsync(
                    userId,
                    jobId);

            return Ok(result);
        }
    }
}