

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
       
    }
}
