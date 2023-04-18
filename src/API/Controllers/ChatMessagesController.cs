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
    [Route("api/v{version:apiVersion}/chat-messages")]
    public class ChatMessagesController : ControllerBase
    {
        private readonly IServiceManager _services;
        public ChatMessagesController(IServiceManager services)
        {
            _services = services;
        }

        /// <summary>
        /// Endpoint to create a chat
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ChatMessageResponseDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateChatMessage(ChatMessageInputDto input)
        {
            var response = await _services.ChatMessageService.CreateChatMessage(input);

            return CreatedAtAction(null, null, response);
        }

        /// <summary>
        /// Endpoint to get all chat messages
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ChatMessageResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetChatMessages([FromQuery] ChatMessageParameters parameters)
        {
            var response = await _services.ChatMessageService.GetChatMessages(parameters, nameof(GetChatMessages), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete chat message
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteChatMessage(Guid id)
        {
            await _services.ChatMessageService.DeleteChatMessage(id);

            return NoContent();
        }

        // <summary>
        /// Endpoint to get all chat hitories
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ChatHistoryResponseDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetChatHistories([FromQuery] ChatHistoryParameters parameters)
        {
            var response = await _services.ChatMessageService.GetChatHistories(parameters, nameof(GetChatMessages), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete chat history
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpDelete("history")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteChatHistory(DeleteChatMessageDto input)
        {
            await _services.ChatMessageService.DeleteChatHistories(input);

            return NoContent();
        }

        /// <summary>
        /// Endpoint to upload media contents
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("uploads")]
        [ProducesResponseType(typeof(SuccessResponse<string[]>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadMediaContent([FromForm] MediaContentDto input)
        {
            var response = await _services.ChatMessageService.UploadChatMediaContent(input);

            return Ok(response);
        }
    }
}
