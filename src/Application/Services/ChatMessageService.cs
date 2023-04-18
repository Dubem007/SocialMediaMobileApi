using Application.Contracts;
using Application.Helpers;
using Application.Validations;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Contracts;
using Infrastructure.Utils.AWS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IWebHelper _webHelper;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IAwsS3Client _awsS3Client;
        public ChatMessageService(IRepositoryManager repository, IMapper mapper, IWebHelper webHelper, IAwsS3Client awsS3Client)
        {
            _repository = repository;
            _mapper = mapper;
            _webHelper = webHelper;
            _awsS3Client = awsS3Client;
        }

        public async Task<SuccessResponse<ChatMessageResponseDto>> CreateChatMessage(ChatMessageInputDto input)
        {
            var senderId = _webHelper.User().UserId;
            if (senderId == input.RecipientId)
                throw new RestException(HttpStatusCode.BadRequest, "You cannot send a message to yourself");
            
            await GetChatMembers(senderId, input.RecipientId);

            var chatMessage = _mapper.Map<ChatMessage>(input);
            chatMessage.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            chatMessage.CreatedById = senderId;

            await CreateChatHistory(input, chatMessage);
            await _repository.ChatMessage.CreateAsync(chatMessage);


            await _repository.SaveChangesAsync();

            var response = _mapper.Map<ChatMessageResponseDto>(chatMessage);

            return new SuccessResponse<ChatMessageResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        private async Task CreateChatHistory(ChatMessageInputDto input, ChatMessage chatMessage)
        {
            var senderId = _webHelper.User().UserId;
            var chatHistory = await _repository.ChatHistory.FindByCondition(x =>
                            (x.SenderId == senderId && x.RecipientId == input.RecipientId) ||
                            (x.RecipientId == senderId && x.SenderId == input.RecipientId), false).FirstOrDefaultAsync();
            
            if (chatHistory is null)
            {
                var chatHistoryEntity = new ChatHistory
                {
                    SenderId = senderId,
                    RecipientId = input.RecipientId
                };
                await _repository.ChatHistory.CreateAsync(chatHistoryEntity);
                chatMessage.ChatHistoryId = chatHistoryEntity.Id;
                chatMessage.SenderId = senderId;

                return;
            }

            chatMessage.ChatHistoryId = chatHistory.Id;
            chatMessage.SenderId = senderId;
        }

        public async Task<PagedResponse<IEnumerable<ChatMessageResponseDto>>> GetChatMessages(ChatMessageParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var senderId = _webHelper.User().UserId;
            await GetChatMembers(senderId, parameters.RecipientId);

            var chatMessagesQuery = _repository.ChatMessage.FindByCondition(x => 
                (x.SenderId == senderId && x.RecipientId == parameters.RecipientId) || 
                (x.SenderId == parameters.RecipientId && x.RecipientId == senderId), false);

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim().ToLower();
                chatMessagesQuery = chatMessagesQuery.Where(x => x.Text.ToLower().Contains(search));
            }

            var chatMessageDtoQuery = chatMessagesQuery.Select(x => new ChatMessageResponseDto
            {
                Id = x.Id,
                RecipientId = x.RecipientId,
                Text = x.Text,
                TimeStamp = x.TimeStamp,
                Media = x.Media,
                MediaTextContent = x.MediaTextContent != null ? new MediaTextContentDto
                {
                    Text = x.MediaTextContent.Text,
                    Media = x.MediaTextContent.Media
                } : null,
                CreatedAt = x.CreatedAt
            }).OrderByDescending(x => x.TimeStamp);            

            var pagedUserMembersDto = await PagedList<ChatMessageResponseDto>.Create(chatMessageDtoQuery, parameters.PageNumber, parameters.PageSize);
            var dynamicParameters = PageUtility<ChatMessageResponseDto>.GenerateResourceParameters(parameters, pagedUserMembersDto);
            var page = PageUtility<ChatMessageResponseDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedUserMembersDto, urlHelper);

            return new PagedResponse<IEnumerable<ChatMessageResponseDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedUserMembersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<ChatHistoryResponseDto>>> GetChatHistories(ChatHistoryParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var senderId = _webHelper.User().UserId;
            var sender = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == senderId, false);
            if (sender is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var chatHistoryQuery = _repository.ChatHistory
                .FindByCondition(x => (x.SenderId == senderId || x.RecipientId == senderId), false)
                .Include(x => x.Sender)
                .Include(x => x.ChatMessages);

            var userMemberQuery = _repository.UserMember.QueryAll();

            var chatHistories = from chatHistory in chatHistoryQuery
                                join userMember in userMemberQuery on chatHistory.RecipientId equals userMember.UserId
                                select new ChatHistoryResponseDto
                                {
                                    Id = chatHistory.Id,
                                    SenderId = chatHistory.Recipient.Id == senderId ? chatHistory.SenderId : chatHistory.Recipient.Id,
                                    RecipientId = chatHistory.Recipient.Id == senderId ? chatHistory.RecipientId : chatHistory.SenderId,
                                    FistName = chatHistory.Recipient.Id == senderId ? chatHistory.Sender.FirstName : chatHistory.Recipient.FirstName,
                                    LastName = chatHistory.Recipient.Id == senderId ? chatHistory.Sender.LastName : chatHistory.Recipient.LastName,
                                    SenderImageUrl = chatHistory.Recipient.Id == senderId ? chatHistory.Sender.ImageUrl : chatHistory.Recipient.ImageUrl,
                                    LastMassage = chatHistory.ChatMessages.OrderByDescending(x => x.CreatedAt).FirstOrDefault().Text,
                                    LastMessageDate = chatHistory.ChatMessages.OrderByDescending(x => x.CreatedAt).FirstOrDefault().TimeStamp,
                                    ProfessionalField = userMember.ProfessionalField,
                                    CreatedAt = chatHistory.ChatMessages.OrderByDescending(x => x.CreatedAt).FirstOrDefault().CreatedAt
                                };

            chatHistories = chatHistories.OrderByDescending(x => x.CreatedAt);

            if (!string.IsNullOrEmpty(parameters.Search))
            {
                var search = parameters.Search.ToLower().Trim();
                chatHistories = chatHistories.Where(x => x.FistName.ToLower().Contains(search) || x.LastName.ToLower().Contains(search) || x.ProfessionalField.ToLower().Contains(search));
            }

            var pagedUserMembersDto = await PagedList<ChatHistoryResponseDto>.Create(chatHistories, parameters.PageNumber, parameters.PageSize);
            var dynamicParameters = PageUtility<ChatHistoryResponseDto>.GenerateResourceParameters(parameters, pagedUserMembersDto);
            var page = PageUtility<ChatHistoryResponseDto>.CreateResourcePageUrl(dynamicParameters, actionName, pagedUserMembersDto, urlHelper);

            return new PagedResponse<IEnumerable<ChatHistoryResponseDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedUserMembersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task<SuccessResponse<object>> DeleteChatHistories(DeleteChatMessageDto input)
        {
            await GetChatMembers(input.SenderId, input.RecipientId);
            var chatHistory = await _repository.ChatHistory.FindByCondition(x =>
                (x.SenderId == input.SenderId && x.RecipientId == input.RecipientId), true)
                .Include(x => x.ChatMessages)
                .FirstOrDefaultAsync();

            if (chatHistory is null)
                throw new RestException(HttpStatusCode.NotFound, "Chat history not found");

            _repository.ChatMessage.DeleteRange(chatHistory.ChatMessages);
            _repository.ChatHistory.Delete(chatHistory);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.DeleteSuccessResponse,
                Data = null
            };
        }

        public async Task<SuccessResponse<object>> DeleteChatMessage(Guid id)
        {
            var chatMessage = await _repository.ChatMessage.FirstOrDefaultAsync(x => x.Id == id, false);
            if (chatMessage is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ChatMessageNotFound);

            _repository.ChatMessage.Delete(chatMessage);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.DeleteSuccessResponse,
                Data = null
            };
        }

        public async Task<SuccessResponse<string>> UploadChatMediaContent(MediaContentDto contentDto)
        {
            var urls = await UploadMedias(contentDto.Files);

            return new SuccessResponse<string>
            {
                Message = "Successfully uploaded",
                Data = urls
            };
        }

        private async Task GetChatMembers(Guid senderId, Guid recipientId)
        {
            var sender = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == senderId, false);
            if (sender is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var recipient = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == recipientId, false);
            if (recipient is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);
        }

        private async Task<string> UploadMedias(IFormFile files)
        {
            var mediaUrl = await _awsS3Client.UploadFileAsync(files);

            return mediaUrl;
        }
    }
}
