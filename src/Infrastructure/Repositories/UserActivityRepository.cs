using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(AppDbContext context) : base(context)
        {

        }
    }
}
