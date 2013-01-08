using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class ResponseHolder
    {  
        public bool Success { get; set; }
        public String Message { get; set; }
        public String ErrorCode { get; set; }
        public List<zObject> Objects { get; set; }
        public String Id { get; set; }
    }
}
