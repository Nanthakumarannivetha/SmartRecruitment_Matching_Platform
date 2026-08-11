using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment.API.Interfaces.Services;
using SmartRecruitment_Project.Helpers;
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

    [HttpGet("discover")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<List<JobMatchDto>>> GetOpenJobs(
        [FromQuery] JobSearchQueryDto query)
    {
        var userId = User.GetUserId();

        var result =
            await _jobDiscoveryService.GetOpenJobsAsync(
                userId,
                query);

        return Ok(result);
    }

    [HttpGet("{jobId:int}/match")]
    [Authorize(Roles = nameof(UserRole.JobSeeker))]
    public async Task<ActionResult<JobMatchDto>> GetJobMatch(
        int jobId)
    {
        var userId = User.GetUserId();

        var result =
            await _jobDiscoveryService.GetJobByIdAsync(
                userId,
                jobId);

        return Ok(result);
    }
}