using Domain.Common;

namespace Domain.Entities
{
    public class UserConnection : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Guid ConnectedMemberId { get; set; }
        public UserMember ConnectedMember { get; set; }
        public UserMember Member { get; set; }
    }
}
