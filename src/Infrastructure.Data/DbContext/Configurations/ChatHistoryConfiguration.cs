using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ChatHistoryConfiguration : IEntityTypeConfiguration<ChatHistory>
    {
        public void Configure(EntityTypeBuilder<ChatHistory> builder)
        {
            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Recipient)
               .WithMany()
               .HasForeignKey(x => x.RecipientId)
               .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.ChatMessages)
                .WithOne(x => x.ChatHistory)
                .HasForeignKey(x => x.ChatHistoryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
