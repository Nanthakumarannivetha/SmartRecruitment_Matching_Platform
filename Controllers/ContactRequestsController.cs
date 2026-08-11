using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.DTOs.ContactRequests;
using SmartRecruitment_Project.Helpers;
using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/contact-requests")]
    [Authorize]
    public class ContactRequestsController : ControllerBase
    {
        private readonly IContactRequestService _contactRequestService;

        public ContactRequestsController(
            IContactRequestService contactRequestService)
        {
            _contactRequestService = contactRequestService;
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<ActionResult<ContactRequestDto>> Create(
            [FromBody] CreateContactRequestDto dto)
        {
            var userId = User.GetUserId();

            var result =
                await _contactRequestService.CreateAsync(
                    userId,
                    dto);

            return Ok(result);
        }

        [HttpGet("mine")]
        [Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<List<ContactRequestDto>>> GetMine()
        {
            var userId = User.GetUserId();

            var result =
                await _contactRequestService.GetMineAsync(userId);

            return Ok(result);
        }

        [HttpPatch("{id:int}/respond")]
        [Authorize(Roles = "JobSeeker")]
        public async Task<ActionResult<ContactRequestDto>> Respond(
            int id,
            [FromBody] RespondContactRequestDto dto)
        {
            var userId = User.GetUserId();

            var result =
                await _contactRequestService.RespondAsync(
                    userId,
                    id,
                    dto);

            return Ok(result);
        }
    }
}