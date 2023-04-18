using Application.Contracts;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using System.Net;

namespace API.Controllers
{
   
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly IServiceManager _service;

        public NotificationController(IServiceManager service)
        {
            _service = service;
        }
        /// <summary>
        /// Endpoint to create a chat
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(NotificationDTO), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateNotification(NotificationCreateDTO message)
        {
            var response =await _service.NotificationService.CreateNotification(message);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get members notifications
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}", Name = (nameof(GetMembersNotifications)))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<NotificationDTO>>), 200)]
        public async Task<IActionResult> GetMembersNotifications(Guid userId, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.NotificationService.GetMembersNotifications(userId, parameter, nameof(GetMembersNotifications), Url);

            return Ok(response);
        }
        /// <summary>
        /// Endpoint to toggle notification(s)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("toggle")]
        [ProducesResponseType(typeof(SuccessResponse<NotificationDTO>), 200)]
        public async Task<IActionResult> ToggleNotifications([FromBody] IEnumerable<Guid> model)
        {
            var response = await _service.NotificationService.ToggleNotificationStatus(model);

            return Ok(response);
        }
    }
}
