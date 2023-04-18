using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils.Notification
{
    public interface INotificationManager
    {
        void SendNotification(string deviceToken, GoogleNotification gNotification);
    }
}
