using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Notification : AuditableEntity
    {
        public Guid Id { get; set; }
        public bool IsRead { get; set; }
        public Guid RecieverId { get; set; }
        public Guid SenderId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public User Sender { get; set; }
        public User Reciever { get; set; }
    }
}
