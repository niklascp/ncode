using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.Payment
{
    public class PaymentFee
    {
        public string VatGroupCode { get; set; }
        
        public decimal Amount { get; set; }
    }
}
