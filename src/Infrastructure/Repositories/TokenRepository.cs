using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class TokenRepository : Repository<Token>, ITokenRepository
    {
        public TokenRepository(AppDbContext context) : base(context)
        {
        }
    }
}
