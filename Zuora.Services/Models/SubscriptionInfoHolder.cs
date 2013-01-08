using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class SubscriptionInfoHolder
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public Subscription Subscription { get; set; }
        public List<RatePlan> RatePlanList { get; set; }
        public List<RatePlanCharge> RatePlanChargeList { get; set; }
        public List<RatePlanChargeTier> RatePlanChargeTierList { get; set; }
    }
}
