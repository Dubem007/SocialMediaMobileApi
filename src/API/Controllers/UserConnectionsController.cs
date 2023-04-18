using Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/userconnections")]
    public class UserConnectionsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserConnectionsController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> UserConnections([FromQuery] ConnectionsByCityParameter parameters)
        {

            var response = await _service.UserConnectionService.UserConnections(parameters, nameof(UserConnections), Url, parameters.Search, parameters.City);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Disconnect(DisconnectDto input)
        {
            var response = await _service.UserConnectionService.Disconnect(input);
            return Ok(response);
        }
    }
}
