using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    class AccountInvoiceInfoHolder
    {
        public Account InvoiceAccount { get; set; }
        public List<Invoice> InvoiceList { get; set; }
        public decimal? InvoiceTotal { get; set; }
    }
}
