using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruitment_Project.Helpers;
using SmartRecruitment_Project.Interfaces.Services;

namespace SmartRecruitment_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Get logged-in user's notifications
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.GetUserId();

            var result =
                await _notificationService
                    .GetMyNotificationsAsync(userId);

            return Ok(result);
        }

        // Mark notification as read
        [HttpPatch("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(
            int notificationId)
        {
            var userId = User.GetUserId();

            var result =
                await _notificationService.MarkAsReadAsync(
                    userId,
                    notificationId);

            return Ok(result);
        }
    }
}