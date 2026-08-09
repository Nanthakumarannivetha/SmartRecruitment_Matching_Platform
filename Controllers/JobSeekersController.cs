using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.DTOs.JobSeekers;
using SmartRecruitment_Project.Interfaces.Services;
using System.Security.Claims;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/job-seekers")]
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekersController : ControllerBase
    {
        private readonly IJobSeekerService _jobSeekerService;

        public JobSeekersController(IJobSeekerService jobSeekerService)
        {
            _jobSeekerService = jobSeekerService;
        }

        private int CurrentUserId
        {
            get
            {
                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException(
                        "User ID claim not found.");
                }

                return int.Parse(userIdClaim.Value);
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var profile =
                await _jobSeekerService.GetProfileAsync(CurrentUserId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Job seeker profile not found."
                });
            }

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(
            UpdateJobSeekerProfileDto dto)
        {
            var profile =
                await _jobSeekerService.UpdateProfileAsync(
                    CurrentUserId,
                    dto);

            return Ok(profile);
        }

        [HttpPut("me/skills")]
        public async Task<IActionResult> UpdateSkills(
            UpdateJobSeekerSkillsDto dto)
        {
            var profile =
                await _jobSeekerService.UpdateSkillsAsync(
                    CurrentUserId,
                    dto);

            return Ok(profile);
        }

        [HttpPost("me/cv")]
        public async Task<IActionResult> UploadCv(
            IFormFile file)
        {
            var result =
                await _jobSeekerService.UploadCvAsync(
                    CurrentUserId,
                    file);

            return Ok(result);
        }

        [HttpGet("me/cv")]
        public async Task<IActionResult> GetCv()
        {
            var result =
                await _jobSeekerService.GetCvAsync(
                    CurrentUserId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "CV not found."
                });
            }

            return File(
                result.Value.Stream,
                result.Value.ContentType,
                result.Value.FileName);
        }
    }
}