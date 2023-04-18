using Domain.Common;
using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ChatMessage : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public Guid ChatHistoryId { get; set; }
        public string[] Media { get; set; }

        [Column(TypeName = "jsonb")]
        public MediaTextContent MediaTextContent { get; set; }
        public string Text { get; set; }
        public string TimeStamp { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public ChatHistory ChatHistory { get; set; }
    }

    public class MediaTextContent
    {
        public string Media { get; set; }
        public string Text { get; set; }
    }
}
