using Application.Contracts;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UsersController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="userForRegistrationDto"></param>
        /// <returns>New User</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userForRegistrationDto)
        {
            var result = await _service.UsersService.RegisterUser(userForRegistrationDto);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Sends verification code to a new member
        /// </summary>
        /// <param name="email"></param>
        /// <returns>String message</returns>

        [HttpPost("Send-invite-Code")]
        public async Task<ActionResult> SendVerificationCode(string email)
        {
            var result = await _service.UsersService.SendInviteCode(email);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// confirms the verification code sent the member
        /// </summary>
        /// <param name="code"></param>
        /// <param name="email"></param>
        /// <returns>string message</returns>
        [HttpPost("confirm-code")]
        public async Task<ActionResult> ConfirmInviteCode(string code, string email)
        {
            var result = await _service.UsersService.ConfirmInviteCode(code, email);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update a users profile
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns>string message</returns>
        [HttpPatch("update-profile")]
        public async Task<ActionResult> UpdateUser(string userId, UpdateUserDto model)
        {
            var result = await _service.UsersService.UpdateProfile(userId, model);
            return StatusCode(result.StatusCode, result);
        }


    }
}
