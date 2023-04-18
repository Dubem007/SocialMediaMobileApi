using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts
{
    public interface IUserConnectionService
    {
        Task<SuccessResponse<string>> Disconnect(DisconnectDto input);
        Task<PagedResponse<IEnumerable<ConnectionsDto>>> UserConnections(ConnectionsByCityParameter parameters, string actionName, IUrlHelper urlHelper, string search, string city);
    }
}