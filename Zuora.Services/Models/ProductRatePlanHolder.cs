using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class ProductRatePlanHolder
    {
        public ProductRatePlan ProductRatePlan { get; set; }
        public List<ProductRatePlanChargeHolder> ProductRatePlanCharges { get; set; }
    }
}
