using Application.Contracts;
using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _service;
        public UsersController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create a User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(UserMemberCreationInputDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> RegisterUser([FromForm] UserMemberCreationInputDto input)
        {
            var response = await _service.UserMemberService.CreateUserMember(input);
            return CreatedAtAction(nameof(GetUserMember), new { id = response.Data.UserMemberId }, response);
        }

        /// <summary>
        /// Endpoint to update a User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [ProducesResponseType(typeof(UserMemberUpdateInputDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateUser([FromForm] UserMemberUpdateInputDto input)
        {
            var response = await _service.UserMemberService.UpdateUserMember(input);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update a User's Photo
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("update-photo")]
        [ProducesResponseType(typeof(UserMemberPhotoUpdateInputDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> UpdateUserMemberPhoto([FromForm] UserMemberPhotoUpdateInputDto parameter)
        {
            var response = await _service.UserMemberService.UpdateUserPhoto(parameter);
            return Ok(response);
        }



        /// <summary>
        /// Endpoint to get all users
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [Authorize]
        [HttpGet(Name = nameof(GetUserMembers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserMemberResponseDto>>), 200)]
        public async Task<IActionResult> GetUserMembers([FromQuery] SearchParameters parameter)
        {
            var response = await _service.UserMemberService.GetUserMembers(parameter, nameof(GetUserMembers), Url);

            return Ok(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserMemberCreationInputDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> GetUserMember(Guid id)
        {
            var response = await _service.UserMemberService.GetUserMemberById(id);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("groups")]
        [ProducesResponseType(typeof(SuccessResponse<GroupUserMembersDto>), 200)]
        public async Task<IActionResult> GetMembersGroup()
        {
            var response = await _service.UserMemberService.MembersByGroupCountAsync();
            return Ok(response);

        }

        [AllowAnonymous]
        [HttpGet("group/{Id}")]
        [ProducesResponseType(typeof(SuccessResponse<GroupUserMembersDto>), 200)]
        public async Task<IActionResult> GetMembersGroupById(Guid Id)
        {
            var response = await _service.UserMemberService.MembersGroupByIdAsync(Id);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("Professions")]
        [ProducesResponseType(typeof(SuccessResponse<ListOfProfessionsDto>), 200)]
        public async Task<IActionResult> AllProfessionalFields()
        {
            var response = await _service.UserMemberService.GetAllProfessionalFields();
            return Ok(response);

        }

        /// <summary>
        /// Endpoint to get all users within user professional field
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("professional-field")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserMemberResponseDto>>), 200)]
        public async Task<IActionResult> GetMembersByUser([FromQuery] SearchUserMembersByUserParameters parameter)
        {

            var response = await _service.UserMemberService.GetUserMembersForUser(parameter, SearchUserOption.ProfessionalField.ToString(), nameof(GetMembersByUser), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all users within user professional field
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("member-suggestions")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserMemberResponseDto>>), 200)]
        public async Task<IActionResult> GetMembersSuggestionsForUser()
        {

            var response = await _service.UserMemberService.GetUserMembersSuggestionsForUser(nameof(GetMembersByUser), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get count per locations 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("count-per-Location")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<BreakdownDto>>), 200)]
        public async Task<IActionResult> GetAllByLocations()
        {

            var response = await _service.UserMemberService.GetAllByTheLocationAsync(nameof(GetAllByLocations), Url);

            return Ok(response);
        }


        /// <summary>
        /// Endpoint to get all users within user recognition year
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("recognition-year")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserMemberResponseDto>>), 200)]
        public async Task<IActionResult> GetUserMembersByUser([FromQuery] SearchUserMembersByUserParameters parameter)
        {

            var response = await _service.UserMemberService.GetUserMembersForUser(parameter, SearchUserOption.RecognitionYear.ToString(), nameof(GetUserMembersByUser), Url);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("per-location")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserMemberResponseDto>>), 200)]
        public async Task<IActionResult> GetUserMembersPerLocation([FromQuery] SearchUserMembersBylocationParameters parameter)
        {

            var response = await _service.UserMemberService.GetUsersPerLocationAsync(parameter, parameter.Search, nameof(GetUserMembersByUser), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all users within user location 
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("locations")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserMemberResponseDto>>), 200)]
        public async Task<IActionResult> GetUserMemberByUser([FromQuery] SearchUserMembersByUserParameters parameter)
        {

            var response = await _service.UserMemberService.GetUserMembersForUser(parameter, SearchUserOption.Location.ToString(), nameof(GetUserMemberByUser), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to location 
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("location-predeictions")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<Predictions>>), 200)]
        public async Task<IActionResult> GetlocationPredeictions([FromQuery] MembersByLocationParameters parameter)
        {

            var response = await _service.UserMemberService.LocationPredictions(parameter, nameof(GetUserMemberByUser), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to location 
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("longitude-latitude")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<LocationDto>>), 200)]
        public async Task<IActionResult> Getlongitudeandlatitude([FromQuery] LocationByPlaceIdParameters parameter)
        {

            var response = await _service.UserMemberService.LongitudeAndLatitlude(parameter, nameof(GetUserMemberByUser), Url);

            return Ok(response);
        }
        /// <summary>
        /// Endpoint to create device token
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [Authorize]
        [HttpPost("devicetoken")]
        [ProducesResponseType(typeof(SuccessResponse<DeviceTokenDto>), 200)]
        public async Task<IActionResult> DeviceToken (DeviceTokenCreateDto model)
        {
            var response = await _service.UserMemberService.CreateDeviceToken(model);

            return Ok(response);
        }


    }
}

