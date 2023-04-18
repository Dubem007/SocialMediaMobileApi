using Application.Contracts;
using Application.Helpers;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using System.Net;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auths")]
    public class AuthsController : ControllerBase
    {
        private readonly IServiceManager _service;
        public AuthsController(IServiceManager service)
        {
            _service = service;
        }

        // <summary>
        /// Endpoint to login a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuccessResponse<UserLoginResponse>), 200)]
        public async Task<IActionResult> LoginUser(UserLoginDTO model)
        {
            var response = await _service.AuthenticationService.Login(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to generate a new access and refresh token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(SuccessResponse<RefreshTokenResponse>), 200)]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO model)
        {
            var response = await _service.AuthenticationService.GetRefreshToken(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to initializes password reset
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDTO model)
        {
            var response = await _service.AuthenticationService.ResetPassword(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var response = await _service.AuthenticationService.ChangePassword(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to verify token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verify-token")]
        [ProducesResponseType(typeof(SuccessResponse<GetConifrmedTokenUserDto>), 200)]
        public async Task<IActionResult> VerifyToken(VerifyTokenDTO model)
        {
            var response = await _service.AuthenticationService.ConfirmToken(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to set password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("set-password")]
        [ProducesResponseType(typeof(SuccessResponse<GetSetPasswordDto>), 200)]
        public async Task<IActionResult> SetPassword([FromForm] SetPasswordDTO model)
        {
            var response = await _service.AuthenticationService.SetPassword(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to send otp
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("send-otp")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> SendToken([FromBody] SendTokenInputDto input)
        {
            var response = await _service.AuthenticationService.SendToken(input);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to confirm otp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("confirm-otp")]
        [ProducesResponseType(typeof(SuccessResponse<ReferenceTokenResponseDto>), 200)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyTokenDTO model)
        {
            var response = await _service.AuthenticationService.VerifyOtp(model);

            return Ok(response);
        }

        
    }
}