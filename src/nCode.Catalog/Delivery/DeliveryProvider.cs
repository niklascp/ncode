using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Globalization;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Delivery
{
    public abstract class DeliveryProvider : ProviderBase
    {
        /// <summary>
        /// Gets a IDeliveryTypeSetupControl used to configure types provided by this Delivery Provider.
        /// </summary>
        public abstract IDeliveryTypeSetupControl GetDeliveryTypeSetupControl();

        /// <summary>
        /// Gets a IDeliveryTypeCheckoutControl used to collect additional information on delivery types provided by this provider doing Checkout.
        /// Returns null if no additional information is to be collected.
        /// </summary>
        public virtual IDeliveryTypeCheckoutControl GetDeliveryTypeCheckoutControl()
        {
            return null;
        }

        /// <summary>
        /// Determines whether the specified Delivery Type (provided by this provider) is available.
        /// </summary>
        public abstract bool IsAvailable(CatalogModel model, Basket basket, DeliveryType deliveryType);

        /// <summary>
        /// Gets the Code of the Vat Group used for the Freight Amount.
        /// </summary>
        public abstract string GetFreightVatGroupCode(CatalogModel model, Basket basket, DeliveryType deliveryType);

        /// <summary>
        /// Gets the Freight Amount.
        /// </summary>
        public abstract decimal GetFreightAmount(CatalogModel model, Basket basket, DeliveryType deliveryType);

        /// <summary>
        /// Gets a describing text string used when selectiong Delivery Type doing Checkout.
        /// </summary>
        public virtual string GetDisplayString(CatalogModel model, DeliveryType deliveryType)
        {
            var localization = deliveryType.Localizations.SingleOrDefault(x => x.Culture == CultureInfo.CurrentUICulture.Name);

            if (localization == null)
                localization = deliveryType.Localizations.Single(x => x.Culture == null);

            return localization.Title;
        }

        /// <summary>
        /// Gets a describing text string used on Checkout Summery.
        /// </summary>
        public virtual string GetSummeryString(CatalogModel model, Basket basket, DeliveryType deliveryType)
        {
            return GetDisplayString(model, deliveryType);
        }

        /// <summary>
        /// Gets a describing text string used on Orders.
        /// </summary>
        public virtual string GetOrderString(CatalogModel model, Order order, DeliveryType deliveryType) 
        {
            var localization = deliveryType.Localizations.SingleOrDefault(x => x.Culture == order.Culture);

            if (localization == null)
                localization = deliveryType.Localizations.Single(x => x.Culture == null);

            return localization.Title;
        }
    }
}
