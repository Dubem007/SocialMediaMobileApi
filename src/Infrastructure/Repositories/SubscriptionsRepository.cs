using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class SubscriptionsRepository : Repository<Subscription>, ISubscriptionsRepository
    {
        public SubscriptionsRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
