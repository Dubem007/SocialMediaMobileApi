using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class InitialMemberRepository : Repository<InitialMember>, IInitialMemberRepository
    {
        public InitialMemberRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
