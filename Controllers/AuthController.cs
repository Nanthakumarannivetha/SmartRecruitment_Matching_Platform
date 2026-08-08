using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.DTOs.Auth;
using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ==========================================
        // Job Seeker Registration
        // ==========================================
        [AllowAnonymous]
        [HttpPost("register/job-seeker")]
        public async Task<IActionResult> RegisterJobSeeker(
            JobSeekerRegisterDto dto)
        {
            var result =
                await _authService.RegisterJobSeekerAsync(dto);

            return Ok(result);
        }

        // ==========================================
        // Employer Registration
        // ==========================================
        [AllowAnonymous]
        [HttpPost("register/employer")]
        public async Task<IActionResult> RegisterEmployer(
            EmployerRegisterDto dto)
        {
            var result =
                await _authService.RegisterEmployerAsync(dto);

            return Ok(result);
        }

        // ==========================================
        // Login
        // ==========================================
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            var result =
                await _authService.LoginAsync(dto);

            return Ok(result);
        }
    }
}