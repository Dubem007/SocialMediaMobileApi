using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class PremiumPlansRepository : Repository<PremiumPlan>, IPremiumPlansRepository
    {
        public PremiumPlansRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
