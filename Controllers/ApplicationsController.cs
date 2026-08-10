using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.API.DTOs.Applications;
using SmartRecruitment.API.Interfaces.Services;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.Controllers;

[ApiController]
[Route("api")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(
        IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // ---------------------------------------------------------
    // JOB SEEKER - APPLY FOR A JOB
    // POST: /api/jobs/5/apply
    // ---------------------------------------------------------
    [HttpPost("jobs/{jobId:int}/apply")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<CreateApplicationResponseDto>> Apply(
        int jobId)
    {
        var userId = GetCurrentUserId();

        var result =
            await _applicationService.ApplyAsync(
                userId,
                jobId);

        return Ok(result);
    }

    // ---------------------------------------------------------
    // JOB SEEKER - VIEW OWN APPLICATIONS
    // GET: /api/applications/mine
    // ---------------------------------------------------------
    [HttpGet("applications/mine")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<List<MyApplicationDto>>> GetMine()
    {
        var userId = GetCurrentUserId();

        var result =
            await _applicationService.GetMyApplicationsAsync(
                userId);

        return Ok(result);
    }

    // ---------------------------------------------------------
    // EMPLOYER - VIEW RANKED APPLICANTS
    // GET: /api/jobs/5/applications
    // ---------------------------------------------------------
    [HttpGet("jobs/{jobId:int}/applications")]
    [Authorize(Roles = nameof(UserRole.Employer))]
    public async Task<ActionResult<List<ApplicantRankingDto>>>
        GetRankedApplicants(int jobId)
    {
        var employerUserId = GetCurrentUserId();

        var result =
            await _applicationService.GetRankedApplicantsAsync(
                employerUserId,
                jobId);

        return Ok(result);
    }

    // ---------------------------------------------------------
    // EMPLOYER - UPDATE APPLICATION STATUS
    // PATCH: /api/applications/3/status
    // ---------------------------------------------------------
    [HttpPatch("applications/{applicationId:int}/status")]
    [Authorize(Roles = nameof(UserRole.Employer))]
    public async Task<IActionResult> UpdateStatus(
        int applicationId,
        [FromBody] UpdateApplicationStatusDto dto)
    {
        var employerUserId = GetCurrentUserId();

        await _applicationService.UpdateApplicationStatusAsync(
            employerUserId,
            applicationId,
            dto.Status);

        return Ok(new
        {
            message = "Application status updated successfully."
        });
    }

    // ---------------------------------------------------------
    // EMPLOYER - SECURELY DOWNLOAD APPLICANT CV
    // GET: /api/applications/3/cv
    // ---------------------------------------------------------
    [HttpGet("applications/{applicationId:int}/cv")]
    [Authorize(Roles = nameof(UserRole.Employer))]
    public async Task<IActionResult> GetApplicantCv(
        int applicationId)
    {
        var employerUserId = GetCurrentUserId();

        var result =
            await _applicationService.GetApplicantCvAsync(
                employerUserId,
                applicationId);

        return File(
            result.FileBytes,
            result.ContentType,
            result.FileName);
    }

    // ---------------------------------------------------------
    // READ AUTHENTICATED USER ID FROM JWT
    // ---------------------------------------------------------
    private int GetCurrentUserId()
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdValue) ||
            !int.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException(
                "User ID claim is missing or invalid.");
        }

        return userId;
    }
}