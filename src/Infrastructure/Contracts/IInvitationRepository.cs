using Domain.Entities;

namespace Infrastructure.Contracts
{
    public interface IInvitationRepository : IRepository<Invitation>
    {
        Task<Invitation> GetInvitation(Guid invitationId);
    }
}
