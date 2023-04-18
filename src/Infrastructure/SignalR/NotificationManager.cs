

using Infrastructure.Logger;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace Infrastructure.SignalR
{
    public class NotificationManager:INotificationManager
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConnectionMultiplexer _redis;
        private ILoggerManager _logger;
        public NotificationManager(IHubContext<NotificationHub> hubContext,
            IConnectionMultiplexer redis,
             ILoggerManager logger)
        {
            _hubContext = hubContext;
            _redis = redis;
            _logger = logger;
        }
        public async Task AddSubscriberAsync(string subscriberId, string topic, string clientCallback)
        {
            if (!_redis.IsConnected)
                throw new Exception("Cannot connect to Redis");
           
            await _redis.GetSubscriber().SubscribeAsync(topic, async (channel, value) =>
            {
                await _hubContext.Clients.User(subscriberId).SendAsync(clientCallback, topic);
            });
            _logger.LogInfo($"Subscribed to channel {topic}");
        }

        public async Task PublishAsync(string topic, string message)
        {
            if (!_redis.IsConnected)
                throw new Exception("Cannot connect to Redis");
            await _redis.GetSubscriber().PublishAsync(topic, message);
            _logger.LogInfo($"Published event to channel {topic}");
        }
    }
}
