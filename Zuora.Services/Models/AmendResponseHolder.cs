using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class AmendResponseHolder
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public AmendResult AmendRes { get; set; }
    }
}
