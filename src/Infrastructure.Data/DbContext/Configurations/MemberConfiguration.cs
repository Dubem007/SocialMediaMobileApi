using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<UserMember>
    {
        public void Configure(EntityTypeBuilder<UserMember> builder)
        {
            builder.HasMany(x => x.UserConnections)
                .WithOne(x => x.ConnectedMember)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
