using Application.Contracts;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IServiceManager _service;
        public SubscriptionController(IServiceManager service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(SuccessResponse<SubscriptionResponseDto>), 200)]
        public async Task<IActionResult> GetSubscriptions()
        {
            var response = await _service.Subscriptions.GetAllSubscriptionsAsync();
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpGet("premium-plans")]
        [ProducesResponseType(typeof(SuccessResponse<PremiumPlansResponseDto>), 200)]
        public async Task<IActionResult> GetPremiumPlans()
        {
            var response = await _service.Subscriptions.GetPremiumPlansAsync();
            return Ok(response);
        }

    }
}
