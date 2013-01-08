using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class ProductRatePlanChargeHolder
    {
        public ProductRatePlanCharge ProductRatePlanCharge { get; set; }
        public List<ProductRatePlanChargeTier> ProductRatePlanChargeTiers { get; set; }
    }
}
