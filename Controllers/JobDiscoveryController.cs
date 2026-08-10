using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment.API.Interfaces.Services;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobDiscoveryController : ControllerBase
{
    private readonly IJobDiscoveryService _jobDiscoveryService;

    public JobDiscoveryController(
        IJobDiscoveryService jobDiscoveryService)
    {
        _jobDiscoveryService = jobDiscoveryService;
    }

    // ---------------------------------------------------------
    // JOB SEEKER - DISCOVER / SEARCH OPEN JOBS
    //
    // GET:
    // /api/jobs/discover
    //
    // Optional query examples:
    // /api/jobs/discover?search=developer
    // /api/jobs/discover?location=Colombo
    // /api/jobs/discover?search=developer&location=Colombo
    // ---------------------------------------------------------
    [HttpGet("discover")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<List<JobMatchDto>>> GetOpenJobs(
        [FromQuery] JobSearchQueryDto query)
    {
        var userId = GetCurrentUserId();

        var result =
            await _jobDiscoveryService.GetOpenJobsAsync(
                userId,
                query);

        return Ok(result);
    }

    // ---------------------------------------------------------
    // JOB SEEKER - VIEW ONE JOB WITH MATCH DETAILS
    //
    // GET:
    // /api/jobs/5/match
    // ---------------------------------------------------------
    [HttpGet("{jobId:int}/match")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<JobMatchDto>> GetJobMatch(
        int jobId)
    {
        var userId = GetCurrentUserId();

        var result =
            await _jobDiscoveryService.GetJobByIdAsync(
                userId,
                jobId);

        return Ok(result);
    }

    // ---------------------------------------------------------
    // GET CURRENT USER ID FROM JWT
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