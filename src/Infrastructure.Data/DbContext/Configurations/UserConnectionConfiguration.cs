using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class UserConnectionConfiguration : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder.HasOne(x => x.ConnectedMember)
                 .WithMany(x => x.UserConnections)
                 .HasForeignKey(x => x.ConnectedMemberId)
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
