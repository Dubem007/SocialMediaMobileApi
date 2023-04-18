using Application.Contracts;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/members")]
    public class MembersController : ControllerBase
    {
        private readonly IServiceManager _services;
        public MembersController(IServiceManager services)
        {
            _services = services;
        }

        /// <summary>
        /// Endpoint to get an initial member
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(SuccessResponse<InitialMemberResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetInitialMember([FromQuery] InitialMemberInputDto input)
        {
            var response = await _services.InitialMemberService.GetInitialMemberByEmail(input);
            return Ok(response);
        }

        [HttpGet("reference-token")]
        [ProducesResponseType(typeof(SuccessResponse<GetInitialMemberTokenResponseDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetInitialMember([FromQuery] ReferenceTokenInputDto input)
        {
            var response = await _services.AuthenticationService.GetInitialMemberByReferenceToken(input);

            return Ok(response);
        }

        [HttpPost("upload-members")]
        [ProducesResponseType(typeof(SuccessResponse<UploadInitialMemberResponseDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadBulkInitialMembers([FromForm] UploadInitialMemberInputDto input)
        {
            var response = await _services.InitialMemberService.UploadBulkInitialMembers(input);
            return CreatedAtAction(null, null, response);
        }

        [HttpPost("upload-professions")]
        [ProducesResponseType(typeof(SuccessResponse<UploadProfessionalListInputDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadBulkProfessionalList([FromForm] UploadProfessionalListInputDto input)
        {
            var response = await _services.InitialMemberService.UploadBulkProfessionalList(input);
            return CreatedAtAction(null, null, response);
        }

        [HttpPost("upload-subscriptions")]
        [ProducesResponseType(typeof(SuccessResponse<UploadProfessionalListInputDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadSubscription([FromForm] UploadSubscriptionInputDto input)
        {
            var response = await _services.InitialMemberService.UploadSubscriptions(input);
            return CreatedAtAction(null, null, response);
        }

        [HttpPost("upload-premiumplans")]
        [ProducesResponseType(typeof(SuccessResponse<UploadProfessionalListInputDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadPremiumPlan([FromForm] UploadPremiumPlansInputDto input)
        {
            var response = await _services.InitialMemberService.UploadPremiumPlans(input);
            return CreatedAtAction(null, null, response);
        }

        [HttpPost("upload-membergroups")]
        [ProducesResponseType(typeof(SuccessResponse<UploadProfessionalListInputDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UploadMemberGroups([FromForm] UploadMemberGroupInputDto input)
        {
            var response = await _services.InitialMemberService.UploadMemberGroup(input);
            return CreatedAtAction(null, null, response);
        }

        [HttpGet("connections-nearby")]
        public async Task<IActionResult> ConnectionsNearby([FromQuery] MembersByLocationParameters parameters)
        {
            var response = await _services.UserMember.ConnectionsNearby(parameters, nameof(ConnectionsNearby), Url);

            return Ok(response);
        }
    }
}
