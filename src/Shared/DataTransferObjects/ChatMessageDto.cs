using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DataTransferObjects
{
    public record ChatMessageResponseDto
    {
        public Guid Id { get; set; }
        public Guid RecipientId { get; set; }
        public string[] Media { get; set; }
        public MediaTextContentDto MediaTextContent { get; set; }
        public string Text { get; set; }
        public string TimeStamp { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record ChatHistoryResponseDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string FistName { get; set; }
        public string LastName { get; set; }
        public string SenderImageUrl { get; set; }
        public string LastMassage { get; set; }
        public string LastMessageDate { get; set; }
        public string ProfessionalField { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record GetChatMessageInputDto
    {
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
    }

    public record DeleteChatMessageDto : GetChatMessageInputDto
    {

    }

    public record ChatMemberDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public record MediaContentDto
    {
        public IFormFile Files { get; set; }
    }

    public record MediaTextContentDto
    {
        public string Media { get; set; }
        public string Text { get; set; }
    }

    public record ChatMessageInputDto
    {
        public Guid RecipientId { get; set; }
        public string[] Media { get; set; }
        public MediaTextContentDto MediaTextContent { get; set; }
        public string Text { get; set; }
    }
}
