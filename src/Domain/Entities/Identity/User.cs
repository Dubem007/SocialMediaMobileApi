using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity;

public class User : IdentityUser<Guid>, IAuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageUrl { get; set; }
    public long RegStatus { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public bool IsDeleted { get; set; }    
    public DateTime? DeletedAt { get; set; }    
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedById { get; set; }
    public ICollection<UserActivity> UserActivities { get; set; }
    public string Status { get; set; }
    public DateTime LastLogin { get; set; }
}