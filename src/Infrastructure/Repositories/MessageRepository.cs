using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class MessageRepository : Repository<ChatMessage>, IMessageRepository
    {
        public MessageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
