using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations
{
    public class PremiumPlanConfiguration : IEntityTypeConfiguration<PremiumPlan>
    {
        public void Configure(EntityTypeBuilder<PremiumPlan> builder)
        {
            builder.HasKey(x => x.PremiumId);
        }
    }
}
