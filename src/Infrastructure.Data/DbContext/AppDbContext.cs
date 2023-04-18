using Domain.Common;
using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Data.DbContext.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContext;

public class AppDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var item in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (item.State)
            {
                case EntityState.Modified:
                    item.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Added:
                    item.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                default:
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }

    public override DbSet<User> Users { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<UserMember> UserMembers { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<UserConnection> UserConnections { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<InitialMember> InitialMembers { get; set; }
    public DbSet<MemberGroup> MemberGroups { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<PremiumPlan> PremiumPlans { get; set; }
    public DbSet<Professions> ProfessionalFields { get; set; }
    public DbSet<DeviceToken> DeviceToken { get; set; }
}
