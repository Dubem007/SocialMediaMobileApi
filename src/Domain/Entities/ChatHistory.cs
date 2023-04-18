using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class ChatHistory : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
