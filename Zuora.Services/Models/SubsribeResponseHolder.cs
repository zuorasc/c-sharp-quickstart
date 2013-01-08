using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class SubscribeResponseHolder
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public SubscribeResult SubRes { get; set; }
    }
}
