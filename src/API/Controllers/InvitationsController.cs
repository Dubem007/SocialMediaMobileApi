using Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace API.Controllers
{
    [Route("api/v1/invitations")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public InvitationsController(IServiceManager service)
        {
            _service = service;
        }


        /// <summary>
        /// End point to get a user's invitations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Invitations([FromQuery] ResourceParameters parameters)
        {
            var response = await _service.InvitationService.GetUserInvitations(parameters, nameof(Invitations), Url);
            return Ok(response);
        }


        /// <summary>
        /// End point to accept or reject an invite
        /// </summary>
        /// <param name="invitationId"></param>
        /// <param name="decision"></param>
        /// <returns></returns>


        [HttpPut("invite-decision")]
        public async Task<IActionResult> Accept(InviteDecisionDto model)
        {
            var response = await _service.InvitationService.InviteDecision(model);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> SendInvitation(InviteRequestDto model)
        {
            var response = await _service.InvitationService.SendInvite(model);
            return Ok(response);
        }

    }
}
