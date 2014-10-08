using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Payment
{
    /// <summary>
    /// Inteface for interacting with Payment Provider for a given Payment.
    /// </summary>
    public interface IPaymentEditControl
    {
        PaymentProvider PaymentProvider { get; set; }
        event EventHandler PaymentStatusChanged;
        string OrderNo { get; set; }
    }
}
