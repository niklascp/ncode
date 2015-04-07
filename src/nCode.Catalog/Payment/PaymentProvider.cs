using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Globalization;
using System.Web;

namespace nCode.Catalog.Payment
{
    public abstract class PaymentProvider : ProviderBase
    {
        public abstract IPaymentTypeSetupControl GetPaymentTypeSetupControl();

        public abstract IPaymentEditControl GetPaymentEditControl();

        public virtual PaymentFee GetPaymentFee(CatalogModel model, Basket basket, PaymentType paymentType)
        {
            return null;
        }

        /// <summary>
        /// Determines whether the specified Delivery Type (provided by this provider) is available.
        /// </summary>
        public virtual bool IsAvailable(CatalogModel model, Basket basket, PaymentType paymentType)
        {
            return true;
        }

        public virtual string GetDisplayString(CatalogModel model, PaymentType paymentType)
        {
            var localization = paymentType.Localizations.SingleOrDefault(x => x.Culture == CultureInfo.CurrentUICulture.Name);

            if (localization == null)
                localization = paymentType.Localizations.Single(x => x.Culture == null);

            return localization.Title;
        }

        public virtual string GetSummeryString(CatalogModel model, Basket basket, PaymentType paymentType)
        {
            return GetDisplayString(model, paymentType);
        }

        public virtual string GetOrderString(CatalogModel model, Order order, PaymentType paymentType)
        {
            var localization = paymentType.Localizations.SingleOrDefault(x => x.Culture == order.Culture);

            if (localization == null)
                localization = paymentType.Localizations.Single(x => x.Culture == null);

            return localization.Title;
        }

        public virtual string GetIntructionString(CatalogModel model, Order order, PaymentType paymentType)
        {
            var localization = paymentType.Localizations.SingleOrDefault(x => x.Culture == order.Culture);

            if (localization == null)
                localization = paymentType.Localizations.Single(x => x.Culture == null);

            if (string.IsNullOrEmpty(localization.Instructions))
                return null;

            return string.Format(localization.Instructions, order.OrderNo, order.CurrencyCode, order.Total, order.DueDate);
        }

        public virtual DateTime GetDueDate(CatalogModel model, Order order, PaymentType paymentType)
        {
            return order.InvoiceDate.Value;
        }

        public virtual void OnOrderPlaced(HttpContext httpContext, Order order)
        {
            httpContext.Response.Redirect("Confirmation?OrderNo=" + order.OrderNo);
        }

        public virtual void OnOrderConfirmed(HttpContext httpContext, Order order)
        {
            order.SendConfirmation("OrderConfirm", new string[] { "~/Catalog/Templates/", "~/Admin/Catalog/Templates/" });
        }
    }
}
