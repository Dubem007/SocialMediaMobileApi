using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class ChatHistoryRepository : Repository<ChatHistory>, IChatHistoryRepository
    {
        public ChatHistoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }
    }
}
