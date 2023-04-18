using Domain.Common;

namespace Domain.Entities
{
    public class InitialMember : AuditableEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public int RecognitionYear { get; set; }
    }
}
