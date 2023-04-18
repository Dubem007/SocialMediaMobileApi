using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InvitationRepository : Repository<Invitation>, IInvitationRepository
    {
        private readonly AppDbContext _context;

        public InvitationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Invitation> GetInvitation(Guid invitationId)
        {
            var invitation = await _context.Invitations.Where(x => x.Id == invitationId && x.Status == EInvitationStatus.Pending.ToString()).FirstOrDefaultAsync();
            return invitation;
        }
    }
}
