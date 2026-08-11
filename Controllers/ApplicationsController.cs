using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.API.DTOs.Applications;
using SmartRecruitment.API.Interfaces.Services;
using SmartRecruitment_Project.Helpers;
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

    [HttpPost("jobs/{jobId:int}/apply")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<CreateApplicationResponseDto>> Apply(
        int jobId)
    {
        var userId = User.GetUserId();

        var result =
            await _applicationService.ApplyAsync(
                userId,
                jobId);

        return Ok(result);
    }

    [HttpGet("applications/mine")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<List<MyApplicationDto>>> GetMine()
    {
        var userId = User.GetUserId();

        var result =
            await _applicationService.GetMyApplicationsAsync(
                userId);

        return Ok(result);
    }

    [HttpGet("jobs/{jobId:int}/applications")]
    [Authorize(Roles = nameof(UserRole.Employer))]
    public async Task<ActionResult<List<ApplicantRankingDto>>>
        GetRankedApplicants(int jobId)
    {
        var employerUserId = User.GetUserId();

        var result =
            await _applicationService.GetRankedApplicantsAsync(
                employerUserId,
                jobId);

        return Ok(result);
    }

    [HttpPatch("applications/{applicationId:int}/status")]
    [Authorize(Roles = nameof(UserRole.Employer))]
    public async Task<IActionResult> UpdateStatus(
        int applicationId,
        [FromBody] UpdateApplicationStatusDto dto)
    {
        var employerUserId = User.GetUserId();

        await _applicationService.UpdateApplicationStatusAsync(
            employerUserId,
            applicationId,
            dto.Status);

        return Ok(new
        {
            message = "Application status updated successfully."
        });
    }

    [HttpGet("applications/{applicationId:int}/cv")]
    [Authorize(Roles = nameof(UserRole.Employer))]
    public async Task<IActionResult> GetApplicantCv(
        int applicationId)
    {
        var employerUserId = User.GetUserId();

        var result =
            await _applicationService.GetApplicantCvAsync(
                employerUserId,
                applicationId);

        return File(
            result.FileBytes,
            result.ContentType,
            result.FileName);
    }
}