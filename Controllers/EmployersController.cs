using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.DTOs.Employers;
using SmartRecruitment_Project.Helpers;
using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employer")]
    public class EmployersController : ControllerBase
    {
        private readonly IEmployerService _employerService;

        public EmployersController(
            IEmployerService employerService)
        {
            _employerService = employerService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetUserId();

            var result =
                await _employerService.GetMyProfileAsync(userId);

            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> CreateOrUpdateMyProfile(
            UpdateEmployerProfileDto dto)
        {
            var userId = User.GetUserId();

            var result =
                await _employerService.CreateOrUpdateProfileAsync(
                    userId,
                    dto);

            return Ok(result);
        }
    }
}