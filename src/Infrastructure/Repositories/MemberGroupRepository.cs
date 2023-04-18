using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class MemberGroupRepository : Repository<MemberGroup>, IMemberGroupRepository
    {
        public MemberGroupRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
