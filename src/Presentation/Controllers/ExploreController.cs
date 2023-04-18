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
using Application.DataTransferObjects;

namespace Presentation.Controllers
{
    [Route("api/v1/exploreusers")]
    [ApiController]
    public class ExploreController : ControllerBase
    {
        private readonly IServiceManager _service;
        //private readonly IExploreServices _explore;

        public ExploreController(IServiceManager service)
        {
            _service = service;
            //_explore = explore;
        }

        [HttpGet("Get People Nearby")]
        public async Task<ActionResult> GetPeopleNearby()
        {
            //IEnumerable<ExploreUsers> resut;
            var username = HttpContext.Session.GetString("loginuser");
            var usrdets = _service.exploreServices.GetUserByUsernameAsync(username);
            //var useremail = usrdets.Result.Data.Email;
            var location = usrdets.Result.Data.city;
           // var usrnm = usrdets.Result.Data.Username;
            var result = await _service.exploreServices.GetPeopleByTheUserLocationAsync(location);
            
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get People By Location")]
        public async Task<ActionResult> GetPeopleByLocation(string location)
        {
           
            var result = await _service.exploreServices.GetByTheLocationAsync(location);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get People By Group")]
        public async Task<ActionResult> GetPeopleByGroup(string group)
        {

            var result = await _service.exploreServices.GetByTheGroupAsync(group);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get All By Groups")]
        public async Task<ActionResult> GetAllByGroups()
        {

            var result = await _service.exploreServices.GetByTheGroupAsync();

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get All By Locations")]
        public async Task<ActionResult> GetAllByLocations()
        {

            var result = await _service.exploreServices.GetAllByTheLocationAsync();
            //var nt = result.Data.GroupBy(x => x.city).Select(x => new BreakdownDto (State = x.Key, Counts = x.Sum(X => X.city))).ToList();
        

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get People In My Field")]
        public async Task<ActionResult> GetPeopleInMyField()
        {
           
            var useremail = HttpContext.Session.GetString("loginuser");
            var usr = _service.exploreServices.GetUserByEmailAsync(useremail);
            var field = usr.Result.Data.ProfessionalField;
            //var usrnm = usr.Result.Data.Username;
            var result = await _service.exploreServices.GetByTheFieldAsync(field);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get Count of all Locations")]
        public async Task<ActionResult> GetCountOfAllLocations()
        {
            var result = await _service.exploreServices.GetAllLocationAsync();
            //var States = result.Data.Select(x=>x.city);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Search Users")]
        public async Task<ActionResult> SearchUsers([FromQuery] ResourceParameter parameter)
        {
            var result = await _service.exploreServices.SearchUsers(parameter);
            //var States = result.Data.Select(x=>x.city);

            return StatusCode(result.StatusCode, result);
        }

       
    }
}
