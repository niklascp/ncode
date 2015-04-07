using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.Delivery
{
    public interface IDeliveryTypeCheckoutControl
    {
        void OnContinueCheckout(Basket basket);
    }
}
