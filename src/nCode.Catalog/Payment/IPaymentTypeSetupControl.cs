using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Payment
{
    /// <summary>
    /// Inteface for setting Payment Provider specific data for a given Payment Type.
    /// </summary>
    public interface IPaymentTypeSetupControl
    {
        void Load(PaymentType pt);
        void Save(PaymentType pt);
    }
}
