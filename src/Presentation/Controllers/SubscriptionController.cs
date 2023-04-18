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
using Shared.RequestFeatures;
using Application.Helpers;

namespace Presentation.Controllers
{
    [Route("api/v1/subscriptions")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IServiceManager _service;
        //private readonly IExploreServices _explore;

        public SubscriptionController(IServiceManager service)
        {
            _service = service;
            //_explore = explore;
        }

        [HttpGet("Get All Subscription")]
        public async Task<ActionResult> GetAllSubscriotions()
        {
            
            var result = await _service.SubscriptionsService.GetAllSubscriptionsAsync();
            
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get User Subscriptions")]
        public async Task<ActionResult> GetUserSubscriptions()
        {
            //IEnumerable<ExploreUsers> resut;
            var useremail = HttpContext.Session.GetString("loginuser");
            //string nt = useremail.ToString();
            var usr = _service.exploreServices.GetUserByEmailAsync(useremail);
            var usrnm = usr.Result.Data.Username;
            var result = await _service.SubscriptionsService.GetAllSubscriptionsByUserAsync(usrnm);

            return StatusCode(result.StatusCode, result);
        }


      
       
    }
}
