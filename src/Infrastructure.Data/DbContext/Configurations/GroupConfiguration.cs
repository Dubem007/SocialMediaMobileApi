using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<MemberGroup>
    {
        public void Configure(EntityTypeBuilder<MemberGroup> builder)
        {
            builder.HasKey(x => x.GroupId);
        }
    }
}
