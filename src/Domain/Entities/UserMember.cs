
using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class UserMember : AuditableEntity
    {

        public Guid Id { get; set; }
        public bool IsSubscribed { get; set; }
        public string PrefferedName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string RecognitionYear { get; set; }
        public string ProfessionalField { get; set; }
        public string Location { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }        
        public string SubscriptionPlan { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<UserConnection> UserConnections { get; set; }
    }
}
