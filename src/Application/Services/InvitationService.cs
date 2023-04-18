using Application.Contracts;
using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace Application.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;
        private readonly INotificationService _notification;


        public InvitationService(IRepositoryManager repository, INotificationService notification, IWebHelper webHelper)
        {
            _repository = repository;
            _notification = notification;
            _webHelper = webHelper;
        }

        public async Task<InvitationPagedResponse<IEnumerable<InvitesDto>>> GetUserInvitations(ResourceParameters parameters, string actionName, IUrlHelper urlHelper)
        {
            var userId = _webHelper.User().UserId;
            var member = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == userId, false);
            var invitations = _repository.Invitation.FindByCondition(x => x.MemberId == member.Id && x.Status == EInvitationStatus.Pending.ToString(), false);

            if (invitations is null || invitations.Count() == 0)
            {
                return new InvitationPagedResponse<IEnumerable<InvitesDto>>();

            }

            var connectionsCount = await _repository.UserConnection.CountAsync(x => x.MemberId == member.Id || x.ConnectedMemberId == member.Id);
            var invites = invitations.Select(x => new InvitesDto
            {
                Id = x.Id,
                MemberId = x.RequesterId,
                UserId = x.Requester.UserId,
                FirstName = x.Requester.User.FirstName,
                Country = x.Requester.Country,
                Location = x.Requester.Location,
                Bio = x.Requester.Bio,
                PrefferedName = x.Requester.PrefferedName,
                DateOfBirth = x.Requester.DateOfBirth,
                Email = x.Requester.User.Email,
                LastName = x.Requester.User.LastName,
                ImageUrl = x.Requester.User.ImageUrl,
                ProfessionalField = x.Requester.ProfessionalField
            });

            var pageInvitessDto = await PagedList<InvitesDto>.Create(invites, parameters.PageNumber, parameters.PageSize);
            var dynamicParameters = PageUtility<InvitesDto>.GenerateResourceParameters(parameters, pageInvitessDto);
            var page = PageUtility<InvitesDto>.CreateResourcePageUrl(dynamicParameters, actionName, pageInvitessDto, urlHelper);

            return new InvitationPagedResponse<IEnumerable<InvitesDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pageInvitessDto,
                Meta = new Meta
                {
                    Pagination = page
                },
                Count = connectionsCount
            };

        }

        public async Task<SuccessResponse<string>> InviteDecision(InviteDecisionDto model)
        {
            var userId = _webHelper.User().UserId;

            var invitation = await _repository.Invitation.GetInvitation(model.InvitationId);
            var response = new SuccessResponse<string>();
            if (invitation is null)
            {
                throw new RestException(HttpStatusCode.NotFound, "Invitation not found");
            }
            var member = await _repository.UserMember.Get(x => x.UserId == userId).Include(x => x.User).FirstOrDefaultAsync();
            if (model.IsConnected)
            {
                var connectedMember = await _repository.UserMember.FirstOrDefaultAsync(x => x.Id == model.MemberId, false);
                invitation.Status = EInvitationStatus.Accepted.ToString();
                var connection = new UserConnection
                {
                    MemberId = member.Id,
                    ConnectedMemberId = connectedMember.Id,
                };
                await _repository.UserConnection.CreateAsync(connection);
                _repository.Invitation.Update(invitation);
                await _repository.SaveChangesAsync();
                NotificationCreateDTO notification = new()
                {
                    SenderId = userId,
                    RecieverId = connectedMember.UserId,
                    Type = ENotificationType.RequestAccepted.ToString(),
                    Title = $"You are connected with {member.User.FirstName} {member.User.LastName}",
                    Body = $"{member.User.FirstName} {member.User.LastName} accepted your connection request"
                };
                await _notification.CreateNotification(notification);
                response.Message = "Connected";
                return response;

            }

            invitation.Status = EInvitationStatus.Rejected.ToString();
            _repository.Invitation.Update(invitation);
            await _repository.SaveChangesAsync();
            response.Message = "Rejected";
            return response;

        }

        public async Task<SuccessResponse<string>> SendInvite(InviteRequestDto request)
        {
            var userId = _webHelper.User().UserId;
            var members = _repository.UserMember.FindAll(false);
            var invitations = _repository.Invitation.FindAll(false);
            var userConnections = _repository.UserConnection.FindAll(false);
            var member = members.Include(x => x.User).FirstOrDefault(x => x.UserId == userId);
            if (request.MemberId == member.Id)
            {
                throw new RestException(HttpStatusCode.Forbidden, "You cannot send an invite to your self");
            }

            var connectedMember = members.FirstOrDefault(x => x.Id == request.MemberId);
            if (connectedMember is null)
            {
                throw new RestException(HttpStatusCode.NotFound, "Member not found");
            }


            var inviteExists = invitations.FirstOrDefault(x => (x.MemberId == member.Id || x.RequesterId == member.Id) && (x.RequesterId == request.MemberId || x.MemberId == request.MemberId) && (x.Status == EInvitationStatus.Pending.ToString()));

            var acceptedInvite = invitations.FirstOrDefault(x => (x.MemberId == member.Id || x.RequesterId == member.Id) && (x.RequesterId == request.MemberId || x.MemberId == request.MemberId) && (x.Status == EInvitationStatus.Accepted.ToString()));

            if (inviteExists != null)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Pending Invite");
            }

            if (acceptedInvite != null)
            {
                throw new RestException(HttpStatusCode.BadRequest, "Already a connection");
            }

            var userConns = userConnections.Where(x => x.ConnectedMemberId == member.Id || x.ConnectedMemberId == member.Id).ToList();

            var connected = userConns.Exists(x => x.ConnectedMemberId == request.MemberId || x.MemberId == member.Id);
            if (connected)
            {
                throw new RestException(HttpStatusCode.BadRequest, "You are already connected to this member");
            }

            var response = new SuccessResponse<string>();
            var invitation = new Invitation()
            {
                Id = Guid.NewGuid(),
                MemberId = request.MemberId,
                RequesterId = member.Id,
                Status = EInvitationStatus.Pending.ToString(),
            };

            await _repository.Invitation.CreateAsync(invitation);
            await _repository.SaveChangesAsync();
            NotificationCreateDTO notification = new()
            {
                SenderId = userId,
                RecieverId = connectedMember.UserId,
                Type = ENotificationType.RequestAccepted.ToString(),
                Title = $"You recieved a connection invite",
                Body = $"{member.User.FirstName} {member.User.LastName} sent a connection request"
            };
            await _notification.CreateNotification(notification);
            response.Message = "Invitation Sent";
            return response;

        }
    }
}
