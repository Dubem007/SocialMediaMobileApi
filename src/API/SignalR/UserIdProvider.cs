using Application.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User.Claims.Where(x => x.Type == ClaimTypeHelper.UserId).FirstOrDefault()?.Value;

            return userId;
        }
    }
}
