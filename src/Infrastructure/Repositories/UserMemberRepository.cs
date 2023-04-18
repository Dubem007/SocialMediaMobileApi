using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class UserMemberRepository : Repository<UserMember>, IUserMemberRepository
    {
        public UserMemberRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
