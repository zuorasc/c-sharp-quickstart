using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class ProductHolder
    {
        public Product Product { get; set; }
        public List<ProductRatePlanHolder> ProductRatePlans { get; set; }
    }
}
