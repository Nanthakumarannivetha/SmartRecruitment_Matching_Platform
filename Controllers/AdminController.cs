using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result =
                await _adminService.GetAllUsersAsync();

            return Ok(result);
        }

        [HttpPatch("users/{userId}/activate")]
        public async Task<IActionResult> ActivateUser(
            int userId)
        {
            var result =
                await _adminService.ActivateUserAsync(userId);

            return Ok(result);
        }

        [HttpPatch("users/{userId}/deactivate")]
        public async Task<IActionResult> DeactivateUser(
            int userId)
        {
            var result =
                await _adminService.DeactivateUserAsync(userId);

            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result =
                await _adminService.GetDashboardAsync();

            return Ok(result);
        }
    }
}