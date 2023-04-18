using Application.Contracts;
using Infrastructure.Contracts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Services;
using Microsoft.AspNetCore.Identity;
using Domain.Entities.Identity;

namespace Presentation.Controllers
{
    [Route("api/v1/connectusers")]
    [ApiController]
    public class ConnectUsersController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly UserManager<User> _userManager;
        //private readonly IExploreServices _explore;

        public ConnectUsersController(IServiceManager service, UserManager<User> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet("Get All Connections")]
        public async Task<ActionResult> GetAllConnections()
        {
           
            var result = await _service.ConnectUserService.GetAllConnectionsAsync();

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get All User Connections")]
        public async Task<ActionResult> GetAllUserConnections()
        {
            var username = HttpContext.Session.GetString("loginuser");
            var usr = _service.exploreServices.GetUserByUsernameAsync(username);
            var email = usr.Result.Data.Email;
            var result = await _service.ConnectUserService.GetAllConnectionsByUserAsync(email);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get Connections Nearby")]
        public async Task<ActionResult> GetAllConnectionsNearby()
        {
            var username = HttpContext.Session.GetString("loginuser");
            var usr = _userManager.FindByNameAsync(username);
            var usrdets = usr.Result;
            var usrfull = _service.exploreServices.GetUserByEmailAsync(usrdets.Email);
            var location = usrfull.Result.Data.city;
            var result = await _service.ConnectUserService.GetConnectionsByUserLocationAsync(location, usrdets.Email);

            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("Get All Pending Connections")]
        public async Task<ActionResult> GetAllPendingConnections()
        {
            var username = HttpContext.Session.GetString("loginuser");
            var usr = _userManager.FindByNameAsync(username);
            var useremail = usr.Result.Email;
            var result = await _service.ConnectUserService.GetAllPendingConnectionsByUserAsync(useremail);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Make Friend Requests")]
        public async Task<ActionResult> Makefriendrequests(string Friendemail)
        {
            var username = HttpContext.Session.GetString("loginuser");
            var usr = _userManager.FindByNameAsync(username);
            var usremail = usr.Result.Email;
            var frienddets = _service.exploreServices.GetUserByEmailAsync(Friendemail);
            var friend = frienddets.Result.Data;
            var result = await _service.ConnectUserService.MakeFriendRequest(usremail, friend);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Accept Friend Requests")]
        public async Task<ActionResult> AcceptFriendRequest(string Friendemail)
        {
            var username = HttpContext.Session.GetString("loginuser");
            var usr = _userManager.FindByNameAsync(username);
            var usremail = usr.Result.Email;
            var frienddets = _service.exploreServices.GetUserByEmailAsync(Friendemail);
            var friend = frienddets.Result.Data;
            var result = await _service.ConnectUserService.AcceptFriendRequest(usremail, friend);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Reject Friend Requests")]
        public async Task<ActionResult> RejectFriendRequest(string Friendemail)
        {
            var username = HttpContext.Session.GetString("loginuser");
            var usr = _userManager.FindByNameAsync(username);
            var usremail = usr.Result.Email;
            var frienddets = _service.exploreServices.GetUserByEmailAsync(Friendemail);
            var friend = frienddets.Result.Data;
            var result = await _service.ConnectUserService.RejectFriendRequest(usremail, friend);

            return StatusCode(result.StatusCode, result);
        }
    }
}
