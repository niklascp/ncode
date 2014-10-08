#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web;
using System.Globalization;

namespace nCode.Catalog.Payment.PayPal
{
    public class PayPalPaymentProvider : PaymentProvider
    {
        /* Redirect Url Format for Proxy 
         * {0} Current Hostname
         * {1} Culture Code
         * {2} Order No                             */
        const string returnUrlFormat = "http:/{0}/{1}/catalog/checkout/Confirmation?OrderNo={2}";
        const string cancelUrlFormat = "http:/{0}/{1}/catalog/catalog/checkout/Summary";

        /* Default Values */
        const string apiHost = "www.paypal.com";
        const string apiEndPointUrl = "https://api-3t.paypal.com/nvp";

        public string ApiHost { get; set; }

        public string ApiEndPointUrl { get; set; }

        public string ApiUsername { get; set; }

        public string ApiPassword { get; set; }

        public string ApiSignature { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config["apiHost"] != null)
                ApiHost = config["apiHost"];
            else
                ApiHost = apiEndPointUrl;

            if (config["apiEndPointUrl"] != null)
                ApiEndPointUrl = config["apiEndPointUrl"];
            else
                ApiEndPointUrl = apiEndPointUrl;

            if (config["apiUsername"] != null)
                ApiUsername = config["apiUsername"];
            else
                throw new ProviderException("Paypal provider '" + name + "' is missing 'apiUsername' attribute.");

            if (config["apiPassword"] != null)
                ApiPassword = config["apiPassword"];
            else
                throw new ProviderException("Paypal provider '" + name + "' is missing 'apiPassword' attribute.");

            if (config["apiSignature"] != null)
                ApiSignature = config["apiSignature"];
            else
                throw new ProviderException("Paypal provider '" + name + "' is missing 'apiSignature' attribute.");
        }

        public override IPaymentTypeSetupControl GetPaymentTypeSetupControl()
        {
            return null;
        }

        public override IPaymentEditControl GetPaymentEditControl()
        {
            return null;
        }

        public override void OnOrderPlaced(HttpContext httpContext, Order order)
        {
            var api = new PayPalApi(this);

            var returnUrl = string.Format(returnUrlFormat,
                httpContext.Request.Url.Host,
                CultureInfo.CurrentUICulture.Name.ToLower(),
                order.OrderNo);
            var cancelUrl = string.Format(cancelUrlFormat,
                httpContext.Request.Url.Host,
                CultureInfo.CurrentUICulture.Name.ToLower());

            var address = new PayPalAddress()
            {
                Name = order.Name,
                Address1 = order.Address1,
                Address2 = order.Address2,
                PostalCode = order.PostalCode,
                City = order.City,
                CountryCode = order.CountryCode,

                Email = order.Email,
                Phone = order.Phone
            };

            string token;
            string returnMessage;

            if (api.ShortcutExpressCheckout(order.CurrencyCode, order.Total, returnUrl, cancelUrl, address, out token, out returnMessage))
            {
                httpContext.Response.Redirect(returnMessage);
            }
            else
            {
                throw new ApplicationException(string.Format("Error in Paypal checkout. Returned message: {0}.", returnMessage));
            }
        }

        public override void OnOrderConfirmed(HttpContext httpContext, Order order)
        {
            PayPalApi api = new PayPalApi(this);

            string token = httpContext.Request.QueryString["Token"];
            string payerId = httpContext.Request.QueryString["PayerID"];

            PayPalApiData apiData;
            string returnMessage;

            if (api.ConfirmPayment(order.CurrencyCode, order.Total, token, payerId, out apiData, out returnMessage))
            {
                using (var catalogModel = new CatalogModel())
                {
                    var _order = catalogModel.Orders.Single(x => x.OrderNo == order.OrderNo);
                    _order.TransactionNo = apiData["TRANSACTIONID"];
                    _order.PaymentStatus = PaymentStatus.Confirmed;
                    catalogModel.SubmitChanges();

                    base.OnOrderConfirmed(httpContext, _order);
                }
            }
            else
            {
                Log.WriteEntry(EntryType.Error, "Catalog", "PayPal Confirm Payment", returnMessage);
            }
        }
    }
}
