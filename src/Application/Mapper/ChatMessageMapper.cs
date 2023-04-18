using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Mapper
{
    public class ChatMessageMapper : Profile
    {
        public ChatMessageMapper()
        {
            CreateMap<ChatMessageInputDto, ChatMessage>()
                .AfterMap((src, dest, context) =>
                {
                    dest.MediaTextContent = context.Mapper.Map<MediaTextContentDto, MediaTextContent>(src.MediaTextContent);
                });
            CreateMap<ChatMessage, ChatMessageResponseDto>()
                .AfterMap((src, dest, context) =>
                {
                    dest.MediaTextContent = context.Mapper.Map<MediaTextContent, MediaTextContentDto>(src.MediaTextContent);
                });
        }
    }
}
