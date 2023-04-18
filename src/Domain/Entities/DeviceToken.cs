using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DeviceToken:AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceId { get; set; }
    }
}
