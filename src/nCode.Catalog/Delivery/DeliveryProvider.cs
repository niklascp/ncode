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
        /// Gets a IDeliveryTypeSetupControl to configure types provided by this Delivery Provider.
        /// </summary>
        public abstract IDeliveryTypeSetupControl GetDeliveryTypeSetupControl();

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

        public virtual string GetDisplayString(CatalogModel model, DeliveryType deliveryType)
        {
            var localization = deliveryType.Localizations.SingleOrDefault(x => x.Culture == CultureInfo.CurrentUICulture.Name);

            if (localization == null)
                localization = deliveryType.Localizations.Single(x => x.Culture == null);

            return localization.Title;
        }

        public virtual string GetOrderString(CatalogModel model, Order order, DeliveryType deliveryType) 
        {
            var localization = deliveryType.Localizations.SingleOrDefault(x => x.Culture == order.Culture);

            if (localization == null)
                localization = deliveryType.Localizations.Single(x => x.Culture == null);

            return localization.Title;
        }
    }
}
