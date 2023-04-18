using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts
{
    public interface IChatMessageService : IAutoDependencyService
    {
        Task<SuccessResponse<ChatMessageResponseDto>> CreateChatMessage(ChatMessageInputDto input);
        Task<PagedResponse<IEnumerable<ChatMessageResponseDto>>> GetChatMessages(ChatMessageParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<ChatHistoryResponseDto>>> GetChatHistories(ChatHistoryParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<object>> DeleteChatHistories(DeleteChatMessageDto input);
        Task<SuccessResponse<object>> DeleteChatMessage(Guid id);
        Task<SuccessResponse<string>> UploadChatMediaContent(MediaContentDto contentDto);
    }
}
