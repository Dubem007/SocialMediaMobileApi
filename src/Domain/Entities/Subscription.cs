using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string SubscriptionPlan { get; set; }
    }
}
