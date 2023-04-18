using Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shared.DataTransferObjects;

namespace API.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IServiceManager _services;
        public ChatHub(IServiceManager services)
        {
            _services = services;
        }
       

        public async Task SendChatMessage(ChatMessageInputDto chatMessage)
        {
            var userId = chatMessage.RecipientId.ToString();

            DefaultContractResolver contractResolver = new()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var response = await _services.ChatMessageService.CreateChatMessage(chatMessage);

            var message = JsonConvert.SerializeObject(response.Data, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }

        private async Task CountUnReadNotificationAsync(string userId)
        {
            var memberId = Guid.Parse(userId);
            int count = await _services.NotificationService.GetUnReadNotificationStats(memberId);

            await Clients.User(userId).SendAsync("UnReadNotificationCount", count);
        }
    }
}
