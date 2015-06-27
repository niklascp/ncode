using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.UI;

namespace nCode.Catalog.Delivery.Gls
{
    public class GlsParcelShopDeliveryProvider : Weight.WeightDeliveryProvider
    {
        /// <summary>
        /// Gets a IDeliveryTypeSetupControl to configure types provided by this Delivery Provider.
        /// </summary>
        public override IDeliveryTypeSetupControl GetDeliveryTypeSetupControl()
        {
            return (IDeliveryTypeSetupControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/Delivery/Gls/GlsParcelShopSetupControl.ascx", typeof(Control));
        }

        /// <summary>
        /// Gets a IDeliveryTypeCheckoutControl used to collect additional information on delivery types provided by this provider doing Checkout.
        /// Returns null if no additional information is to be collected.
        /// </summary>
        public override IDeliveryTypeCheckoutControl GetDeliveryTypeCheckoutControl()
        {
            return (IDeliveryTypeCheckoutControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/Delivery/Gls/GlsParcelShopCheckoutControl.ascx", typeof(Control));
        }

        /// <summary>
        /// Gets a describing text string used on Checkout Summery.
        /// </summary>
        public override string GetSummeryString(CatalogModel model, Basket basket, DeliveryType deliveryType)
        {
            if (!string.IsNullOrEmpty(basket.GetProperty("GlsParcelShopNumber", string.Empty)))
            {
                return string.Format("{0}: {1} ({2})", base.GetSummeryString(model, basket, deliveryType), basket.GetProperty("GlsParcelShopName", string.Empty), basket.GetProperty("GlsParcelShopNumber", string.Empty));
            }
            else
            {
                return base.GetSummeryString(model, basket, deliveryType);
            }
        }

        /// <summary>
        /// Gets a describing text string used on Orders.
        /// </summary>
        public override string GetOrderString(CatalogModel model, Order order, DeliveryType deliveryType)
        {
            if (!string.IsNullOrEmpty(order.GetProperty("GlsParcelShopNumber", string.Empty)))
            {
                return string.Format("{0}: {1} ({2})", base.GetOrderString(model, order, deliveryType), order.GetProperty("GlsParcelShopName", string.Empty), order.GetProperty("GlsParcelShopNumber", string.Empty));
            }
            else
            {
                return base.GetOrderString(model, order, deliveryType);
            }
        }

        /// <summary>
        /// Determines whether the specified Delivery Type (provided by this provider) is available.
        /// </summary>
        public override bool IsAvailable(CatalogModel model, Basket basket, DeliveryType deliveryType)
        {
            return basket.UseShippingAddress ? basket.ShippingCountryCode == "DK" : basket.CountryCode == "DK";
        }
    }
}
