using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.HasOne(x => x.Requester)
                .WithMany()
                .HasForeignKey(x => x.RequesterId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
