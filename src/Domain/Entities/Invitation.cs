using Domain.Common;

namespace Domain.Entities
{
    public class Invitation : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Guid RequesterId { get; set; }
        public UserMember Requester { get; set; }
        public string Status { get; set; } 
    }
}
