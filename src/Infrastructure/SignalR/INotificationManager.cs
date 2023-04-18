using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SignalR
{
    public interface INotificationManager
    {
        Task AddSubscriberAsync(string subscriberId, string topic, string clientCallback);
        Task PublishAsync(string topic, string message);
    }
}
