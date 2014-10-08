#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Payment.PayPal
{
    public class PayPalAddress
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
