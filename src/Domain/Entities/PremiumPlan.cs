using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PremiumPlan
    {
        public Guid PremiumId { get; set; }
        public string Duration { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PercentSavings { get; set; }
        public string Savings { get; set; }
    }
}
