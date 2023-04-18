using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserConnectionRepository : Repository<UserConnection>, IUserConnectionRepository
    {
        private readonly AppDbContext _context;

        public UserConnectionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> UserConnectionCount(Guid userId)
        {
            var result = await _context.UserConnections.Where(x => x.MemberId == userId).CountAsync();

            return result;
        }

    }
}
