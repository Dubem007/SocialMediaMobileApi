using Domain.Entities;

namespace Infrastructure.Contracts
{
    public interface IUserConnectionRepository : IRepository<UserConnection>
    {
        Task<int> UserConnectionCount(Guid userId);
    }
}
