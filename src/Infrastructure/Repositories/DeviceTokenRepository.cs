using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class DeviceTokenRepository : Repository<DeviceToken>, IDeviceTokenRepository
    {
        public DeviceTokenRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }
    }
}
