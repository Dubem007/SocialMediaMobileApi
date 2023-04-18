using Application.Contracts;
using Application.DataTransferObjects;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _service;
    private readonly UserManager<User> _userManager;

    public AuthController(IServiceManager service, UserManager<User> userManager)
    {
        _service = service;
        _userManager = userManager;
    }


    /// <summary>
    /// Logs in user with email and password
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Jwt Token</returns>
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDto user)
    {
        var result = await _service.AuthenticationService.Login(user);
        var usr = await _userManager.FindByEmailAsync(user.Email);
        var usrname = usr.Username;
        if (result.Succeeded)
        {
            HttpContext.Session.SetString("loginuser", usrname/*.ToLower()*/);

        }
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// user forgets his/her password, sends a reset code
    /// </summary>
    /// <param name="id"></param>
    /// <param name="email"></param>
    /// <returns>string message</returns>
    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(string id, string email)
    {
        var result = await _service.AuthenticationService.ForgotPassword(id, email);

        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// confirm the reset code sent to the member from the forgot password endpoint
    /// </summary>
    /// <param name="code"></param>
    /// <param name="email"></param>
    /// <returns>string messaage</returns>
    [HttpPost("confirm-reset-code")]
    public async Task<ActionResult> ConfirmResetCode(string code, string email)
    {
        var result = await _service.AuthenticationService.ConfirmResetCode(code, email);

        return StatusCode(result.StatusCode, result);
    }


    /// <summary>
    /// carries out the actual password reset
    /// </summary>
    /// <param name="model"></param>
    /// <returns>string message</returns>
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordDto model)
    {
        var result = await _service.AuthenticationService.ResetPassword(model);

        return StatusCode(result.StatusCode, result);
    }


    /// <summary>
    /// Updates the user password
    /// </summary>
    /// <param name="model"></param>
    /// <returns>string message</returns>
    [HttpPatch("update-password")]
    public async Task<ActionResult<Response<string>>> UpdatePassword([FromBody] UpdatePasswordDto model)
    {
        var result = await _service.AuthenticationService.UpdatePassword(model);
        return StatusCode(result.StatusCode, result);
    }


    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tokenDto"></param>
    /// <returns></returns>
    [HttpPost("refresh-token{id}")]
    public async Task<IActionResult> RefreshToken(string userId, [FromBody] TokenDto tokenDto)
    {
        var token = await _service.AuthenticationService.RefreshToken(userId, tokenDto);
        return Ok(token);
    }
}