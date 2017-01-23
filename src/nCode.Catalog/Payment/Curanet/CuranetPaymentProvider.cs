using System;
using System.Linq;
using System.Web.Compilation;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Globalization;
using nCode.Catalog.Models;

namespace nCode.Catalog.Payment.Curanet
{
    public sealed class CuranetPaymentProvider : PaymentProvider
    {
        const string defaultApiUrl = "https://betaling.curanet.dk/api/customerAPI.php";
        const string defaultPaymentWindowUrl = "https://betaling.curanet.dk/paymentwindow/";
        const string appSettingsFormat = "nCode.Catalog.Payment.Curanet.{0}";

        /// <summary>
        /// Gets the Shop ID for this Curanet Payment Provider.
        /// </summary>
        public string ShopId { get; private set; }

        /// <summary>
        /// Gets the Username for this Curanet Payment Provider. For instance used to interact with the Curanet API.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the Password for this Curanet Payment Provider. For instance used to interact with the Curanet API.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the Api Url for this Curanet Payment Provider. Used as Endpoint Url for the Curanet API.
        /// </summary>        
        public string ApiUrl { get; private set; }

        /// <summary>
        /// Gets the Payment Window Url for this Curanet Payment Provider.
        /// </summary>
        public string PaymentWindowUrl { get; private set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["shopId"] != null)
                ShopId = config["shopId"];
            else
                ShopId = ConfigurationManager.AppSettings[string.Format(appSettingsFormat, "ShopID")]; /* Legacy load from AppSettings. */

            if (config["username"] != null)
                Username = config["username"];
            else
                Username = ConfigurationManager.AppSettings[string.Format(appSettingsFormat, "Username")]; /* Legacy load from AppSettings. */

            if (config["password"] != null)
                Password = config["password"];
            else
                Password = ConfigurationManager.AppSettings[string.Format(appSettingsFormat, "Password")]; /* Legacy load from AppSettings. */

            if (config["apiUrl"] != null)
                ApiUrl = config["apiUrl"];
            /* Legacy load from AppSettings. */
            else if (ConfigurationManager.AppSettings[string.Format(appSettingsFormat, "ApiUrl")] != null)
                ApiUrl = ConfigurationManager.AppSettings[string.Format(appSettingsFormat, "ApiUrl")];
            else
                ApiUrl = defaultApiUrl;

            if (config["paymentWindowUrl"] != null)
                PaymentWindowUrl = config["paymentWindowUrl"];
            else
                PaymentWindowUrl = defaultPaymentWindowUrl;
        }

        public override IPaymentTypeSetupControl GetPaymentTypeSetupControl()
        {
            return (IPaymentTypeSetupControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/Payment/Curanet/PaymentTypeSetupControl.ascx", typeof(Control));
        }

        public override IPaymentEditControl GetPaymentEditControl()
        {
            return (IPaymentEditControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/Payment/Curanet/PaymentEditControl.ascx", typeof(Control));
        }

        public override bool IsAvailable(CatalogModel model, Basket basket, PaymentType paymentType)
        {
            var cardTypes = paymentType.GetProperty<string[]>("CardTypes", new string[0]);

            if (!string.Equals(basket.CurrencyCode, "DKK", StringComparison.OrdinalIgnoreCase) && (cardTypes.Contains("AMEX(DA)") || cardTypes.Contains("AMEX")))
                return false;

            return true;
        }

        public override PaymentFee GetPaymentFee(CatalogModel model, Basket basket, PaymentType paymentType)
        {
            var feeVatGroupCode = paymentType.GetProperty("FeeVatGroupCode", (string)null);
            var baseFee = paymentType.GetProperty("BaseFee", 0m);
            var percentageFee = paymentType.GetProperty("PercentageFee", 0m);

            if (baseFee != 0m || percentageFee != 0m)
            {
                using (var dbContext = new CatalogDbContext())
                {
                    var total = basket.Total;
                    var baseFeeInBasketCurrency = CurrencyController.ConvertAmount(baseFee, fromCurrency: CurrencyController.DefaultCurrency, toCorrency: basket.Currency);
                    var percentageFeeInBasketCurrency = total * percentageFee;
                    var feeInBasketCurrency = baseFeeInBasketCurrency + percentageFeeInBasketCurrency;

                    return new PaymentFee
                    {
                        VatGroupCode = feeVatGroupCode,
                        Amount = VatUtilities.GetDisplayPrice(feeInBasketCurrency, feeVatGroupCode, null, basket.IncludeVat)
                    };
                }
            }
            else
            {
                return null;
            }
        }

        private string GetStatusString(Order order)
        {
            if (string.Equals(order.Culture, "da-DK", StringComparison.InvariantCultureIgnoreCase))
            {
                if (order.PaymentStatus == PaymentStatus.Completed)
                    return string.Format("Gennemført, transaktion: {0}", order.TransactionNo);
                else if (order.PaymentStatus == PaymentStatus.Confirmed)
                    return string.Format("Bekræftet, transaktion: {0}", order.TransactionNo);
                else
                    return string.Format("Ikke bekræftet");
            }
            else
            {
                if (order.PaymentStatus == PaymentStatus.Completed)
                    return string.Format("Completed, transaction: {0}", order.TransactionNo);
                else if (order.PaymentStatus == PaymentStatus.Confirmed)
                    return string.Format("Confirmed, transaction: {0}", order.TransactionNo);
                else
                    return string.Format("Unconfirmed");
            }
        }

        public override string GetOrderString(CatalogModel model, Order order, PaymentType paymentType)
        {
            return string.Format("{0} ({1})", base.GetOrderString(model, order, paymentType), GetStatusString(order));
        }

        public override void OnOrderPlaced(HttpContext httpContext, Order order)
        {
            /* For zero or negative orders we just result to default behaviour. */
            if (order.Total <= 0)
            {
                base.OnOrderPlaced(httpContext, order);
                return;
            }

            /* Validate OrderNo (Curanet only allows numbers for orderno.) */
            int orderNo;
            if (int.TryParse(order.OrderNo, out orderNo))
            {
                httpContext.Response.Redirect($"/{order.Culture.ToLower()}/catalog/checkout/curanet/Payment?OrderNo={orderNo}");
            }
            else
            {
                throw new ApplicationException("OrderNo cannot have pre-/postfix when using the Curanet payment provider.");
            }
        }

        public override void OnOrderConfirmed(HttpContext httpContext, Order order)
        {
            /* For zero or negative orders we just result to default behaviour. */
            if (order.Total <= 0)
            {
                using (var model = new CatalogModel())
                {
                    var _order = model.Orders.Single(x => x.OrderNo == order.OrderNo);
                    if (order.Total == 0)
                    {
                        _order.PaymentStatus = PaymentStatus.Confirmed;
                        model.SubmitChanges();
                    }

                    base.OnOrderConfirmed(httpContext, _order);
                }
                return;
            }

            /* Confirmation are handled by Callback-method. */
        }

        public void OnPaymentPageLoad(HttpContext httpContext, Order order)
        {
            // No-Op
        }

        public void OnPaymentCallback(HttpContext httpContext, Order order)
        {
            using (var model = new CatalogModel())
            {
                var _order = model.Orders.Single(x => x.OrderNo == order.OrderNo);
                _order.PaymentStatus = PaymentStatus.Confirmed;
                _order.TransactionNo = httpContext.Request["transacknum"];
                model.SubmitChanges();

                base.OnOrderConfirmed(httpContext, _order);
            }
        }
    }
}
