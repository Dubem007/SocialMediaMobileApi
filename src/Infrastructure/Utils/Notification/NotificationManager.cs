
using Microsoft.Extensions.Configuration;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Shared.DataTransferObjects.GoogleNotification;
using CorePush.Google;
using Hangfire;

namespace Infrastructure.Utils.Notification
{
    public class NotificationManager: INotificationManager
    {
        private readonly IConfiguration _configuration;

        public NotificationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendNotification(string deviceToken, GoogleNotification gNotification)
        {
            BackgroundJob.Enqueue(() => SendNotificationAsync(deviceToken, gNotification));
        }

        public async Task SendNotificationAsync(string deviceToken, GoogleNotification gNotification)
        {
           
            var fcmNotification = _configuration.GetSection("FcmNotification");
            var senderId = fcmNotification.GetSection("SenderId").Value;
            var serverId = fcmNotification.GetSection("ServerKey").Value;
            FcmSettings settings = new()
            {
                SenderId = senderId,
                ServerKey = serverId
            };
            string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
            HttpClient client = new();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var fcm = new FcmSender(settings, client);
            var response = await fcm.SendAsync(deviceToken, gNotification);
            if (!response.IsSuccess())
                throw new Exception(response.Failure.ToString());

        }
    }
}
