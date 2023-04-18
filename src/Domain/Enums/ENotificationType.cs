using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ENotificationType
    {
        [Description("Connection Request Recieved")]
        RequestRecieved,
        [Description("Connection Request Accepted")]
        RequestAccepted,
        [Description("Added to Group")]
        AddedToGroup,
       [Description("ChatRecieved")]
        ChatRecieved
    }
}
