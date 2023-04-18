using Application.Contracts;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace Application.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IWebHelper _webHelper;

        public UserConnectionService(IRepositoryManager repository, IMapper mapper, IWebHelper webHelper)
        {
            _repository = repository;
            _mapper = mapper;
            _webHelper = webHelper;
        }

        public async Task<PagedResponse<IEnumerable<ConnectionsDto>>> UserConnections(ConnectionsByCityParameter parameters, string actionName, IUrlHelper urlHelper, string search, string city)
        {
            var userId = _webHelper.User().UserId;
            var member = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == userId, false);
            IQueryable<UserConnection> userConnections = _repository.UserConnection.FindByCondition(x => x.MemberId == member.Id || x.ConnectedMemberId == member.Id, false);
            if (city != null)
            {
                userConnections = userConnections.Where(x => x.ConnectedMember.Location == city);
            }

            var connections = userConnections.Where(x => x.ConnectedMemberId != member.Id).Select(x => new ConnectionsDto
            {
                MemberId = x.ConnectedMemberId,
                Country = x.ConnectedMember.Country,
                ProfessionalField = x.ConnectedMember.ProfessionalField,
                ImageUrl = x.ConnectedMember.User.ImageUrl,
                Location = x.ConnectedMember.Location,
                FirstName = x.ConnectedMember.User.FirstName,
                LastName = x.ConnectedMember.User.LastName,
                UserId = x.ConnectedMember.UserId,
                DateOfBirth = x.ConnectedMember.DateOfBirth,
                PrefferedName = x.ConnectedMember.PrefferedName,
                Email = x.ConnectedMember.User.Email,
                Bio = x.ConnectedMember.Bio,
            });

            var connections1 = userConnections.Where(x => x.MemberId != member.Id).Select(x => new ConnectionsDto
            {
                MemberId = x.MemberId,
                Country = x.Member.Country,
                ProfessionalField = x.Member.ProfessionalField,
                ImageUrl = x.Member.User.ImageUrl,
                Location = x.Member.Location,
                FirstName = x.Member.User.FirstName,
                LastName = x.Member.User.LastName,
                UserId = x.Member.UserId,
                DateOfBirth = x.Member.DateOfBirth,
                PrefferedName = x.Member.PrefferedName,
                Email = x.Member.User.Email,
                Bio = x.Member.Bio,
            });

            if (search != null)
            {
                search = search.ToLowerInvariant();
                connections = connections.Where(x => x.FirstName.ToLower().StartsWith(search) || x.LastName.ToLower().StartsWith(search));

                connections1 = connections1.Where(x => x.FirstName.ToLower().StartsWith(search) || x.LastName.ToLower().StartsWith(search));

            }
            var totalConnections = connections.Concat(connections1);

            var pageConnectionsDto = await PagedList<ConnectionsDto>.Create(totalConnections, parameters.PageNumber, parameters.PageSize);
            var dynamicParameters = PageUtility<ConnectionsDto>.GenerateResourceParameters(parameters, pageConnectionsDto);
            var page = PageUtility<ConnectionsDto>.CreateResourcePageUrl(dynamicParameters, actionName, pageConnectionsDto, urlHelper);

            return new PagedResponse<IEnumerable<ConnectionsDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pageConnectionsDto,
                Meta = new Meta
                {
                    Pagination = page
                },
            };

        }

        public async Task<SuccessResponse<string>> Disconnect(DisconnectDto input)
        {
            var userId = _webHelper.User().UserId;
            var member = await _repository.UserMember.FirstOrDefaultAsync(x => x.UserId == userId, false);
            var connection = await _repository.UserConnection.FirstOrDefaultAsync(x => (x.ConnectedMemberId == input.MemberId || x.MemberId == input.MemberId) && (x.ConnectedMemberId == member.Id || x.MemberId == member.Id), false);
            if (connection is null)
            {
                throw new RestException(HttpStatusCode.NotFound, "Connection not found");
            }


            if (input.Decision)
            {
                var invitation = await _repository.Invitation.FirstOrDefaultAsync(x => (x.RequesterId == input.MemberId || x.MemberId == input.MemberId) && (x.RequesterId == member.Id || x.MemberId == member.Id), false);
                _repository.Invitation.Delete(invitation);
                _repository.UserConnection.Delete(connection);
                await _repository.SaveChangesAsync();

                return new SuccessResponse<string>
                {
                    Data = null,
                    Message = "Disconnected",
                    Success = true,
                };
            }

            return new SuccessResponse<string>
            {
                Data = null,
                Message = "Not disconnected",
                Success = true,
            };

        }

        private static Guid MemberIdCheck(Guid LoggedInUser, Guid MemberId, Guid ConnectedMemberId)
        {
            if (MemberId == LoggedInUser)
            {
                return ConnectedMemberId;
            }
            return MemberId;
        }


    }
}
