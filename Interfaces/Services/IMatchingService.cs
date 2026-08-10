using SmartRecruitment.API.DTOs.Jobs;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment.API.Interfaces.Services;

public interface IMatchingService
{
    MatchResultDto CalculateMatch(
        JobSeekerProfile seeker,
        JobVacancy vacancy);
}