using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts
{
    public interface IInvitationService
    {
        Task<InvitationPagedResponse<IEnumerable<InvitesDto>>> GetUserInvitations(ResourceParameters parameters, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<string>> InviteDecision(InviteDecisionDto model);
        Task<SuccessResponse<string>> SendInvite(InviteRequestDto request);
    }
}