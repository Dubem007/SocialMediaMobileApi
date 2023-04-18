using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class ProfessionalFieldConfiguration : IEntityTypeConfiguration<Professions>
    {
        public void Configure(EntityTypeBuilder<Professions> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
