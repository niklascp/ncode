#pragma warning disable 1591

using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Globalization;

namespace nCode.Catalog.Payment.PayPal
{
    public class PayPalApi
    {
        private const string CVV2 = "CVV2";
        private const string SIGNATURE = "SIGNATURE";
        private const string PWD = "PWD";
        private const string ACCT = "ACCT";

        private string Subject = "";
        private string BNCode = "PP-ECWizard";

        private string[] acceptedLocales = new string[] { "US", "GB", "AU", "DE", "FR", "IT", "ES", "JP" };

        //HttpWebRequest Timeout specified in milliseconds 
        private const int Timeout = 5000;
        private static readonly string[] SECURED_NVPS = new string[] { ACCT, CVV2, SIGNATURE, PWD };

        public PayPalPaymentProvider Provider { get; private set; }

        public PayPalApi(PayPalPaymentProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// ShortcutExpressCheckout: The method that calls SetExpressCheckout API.
        /// </summary>
        public bool ShortcutExpressCheckout(string currencyCode, decimal amount, string returnURL, string cancelURL, PayPalAddress shipmentAddress, out string token, out string returnMessage)
        {   
            string locale = acceptedLocales[0];

            foreach (var l in CultureInfo.CurrentUICulture.Name.Split(new char[]{'-'}))
            {
                if (acceptedLocales.Any(x => string.Equals(x, l, StringComparison.InvariantCultureIgnoreCase)))
                    locale = l.ToUpper();
            }
            
            PayPalApiData encoder = new PayPalApiData();
            encoder["METHOD"] = "SetExpressCheckout";
            encoder["RETURNURL"] = returnURL;
            encoder["CANCELURL"] = cancelURL;
            encoder["AMT"] = amount.ToString("f2", CultureInfo.InvariantCulture);
            encoder["PAYMENTACTION"] = "Sale";
            encoder["CURRENCYCODE"] = currencyCode;
            encoder["LOCALECODE"] = locale;

            if (shipmentAddress != null)
            {
                encoder["NOSHIPPING"] = "1";
                encoder["SHIPTONAME"] = shipmentAddress.Name;
                encoder["SHIPTOSTREET"] = shipmentAddress.Address1;
                encoder["SHIPTOSTREET2"] = shipmentAddress.Address2;
                encoder["SHIPTOCITY"] = shipmentAddress.City;
                encoder["SHIPTOSTATE"] = shipmentAddress.State;
                encoder["SHIPTOZIP"] = shipmentAddress.PostalCode;
                encoder["SHIPTOCOUNTRYCODE"] = shipmentAddress.CountryCode;

                encoder["EMAIL"] = shipmentAddress.Email;
                encoder["SHIPTOPHONENUM"] = shipmentAddress.Phone;
            }

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            PayPalApiData decoder = new PayPalApiData();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                token = decoder["TOKEN"];

                returnMessage = "https://" + Provider.ApiHost + "/cgi-bin/webscr?cmd=_express-checkout&token=" + token;
                
                return true;
            }
            else
            {
                token = null;

                returnMessage =
                    "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }

        /*
/// <summary>
/// GetShippingDetails: The method that calls SetExpressCheckout API, invoked from the 
/// Billing Page EC placement
/// </summary>
/// <param name="token"></param>
/// <param ref name="retMsg"></param>
/// <returns></returns>
public bool GetShippingDetails(string token, out string PayerId, out string ShippingAddress, out string retMsg)
{
    PayPalApiCodec encoder = new PayPalApiCodec();
    encoder["METHOD"] = "GetExpressCheckoutDetails";
    encoder["TOKEN"] = token;

    string pStrrequestforNvp = encoder.Encode();
    string pStresponsenvp = HttpCall(pStrrequestforNvp);

    PayPalApiCodec decoder = new PayPalApiCodec();
    decoder.Decode(pStresponsenvp);

    string strAck = decoder["ACK"].ToLower();
    if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
    {
        ShippingAddress = "<table><tr>";
        ShippingAddress += "<td> First Name </td><td>" + decoder["FIRSTNAME"] + "</td></tr>";
        ShippingAddress += "<td> Last Name </td><td>" + decoder["LASTNAME"] + "</td></tr>";
        ShippingAddress += "<td colspan='2'> Shipping Address</td></tr>";
        ShippingAddress += "<td> Name </td><td>" + decoder["SHIPTONAME"] + "</td></tr>";
        ShippingAddress += "<td> Street1 </td><td>" + decoder["SHIPTOSTREET"] + "</td></tr>";
        ShippingAddress += "<td> Street2 </td><td>" + decoder["SHIPTOSTREET2"] + "</td></tr>";
        ShippingAddress += "<td> City </td><td>" + decoder["SHIPTOCITY"] + "</td></tr>";
        ShippingAddress += "<td> State </td><td>" + decoder["SHIPTOSTATE"] + "</td></tr>";
        ShippingAddress += "<td> Zip </td><td>" + decoder["SHIPTOZIP"] + "</td>";
        ShippingAddress += "</tr>";

        return true;
    }
    else
    {
        retMsg = "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
            "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
            "Desc2=" + decoder["L_LONGMESSAGE0"];

        return false;
    }
}
*/

        /// <summary>
        /// ConfirmPayment: The method that calls SetExpressCheckout API, invoked from the 
        /// Billing Page EC placement
        /// </summary>
        public bool ConfirmPayment(string currencyCode, decimal amount, string token, string PayerId, out PayPalApiData decoder, out string returnMessage)
        {
            PayPalApiData encoder = new PayPalApiData();
            encoder["METHOD"] = "DoExpressCheckoutPayment";
            encoder["TOKEN"] = token;
            encoder["PAYMENTACTION"] = "Sale";
            encoder["PAYERID"] = PayerId;
            encoder["CURRENCYCODE"] = currencyCode;
            encoder["AMT"] = amount.ToString("f2", CultureInfo.InvariantCulture);

            string pStrrequestforNvp = encoder.Encode();
            string pStresponsenvp = HttpCall(pStrrequestforNvp);

            decoder = new PayPalApiData();
            decoder.Decode(pStresponsenvp);

            string strAck = decoder["ACK"].ToLower();
            if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
            {
                returnMessage = null;

                return true;
            }
            else
            {
                returnMessage = 
                    "ErrorCode=" + decoder["L_ERRORCODE0"] + "&" +
                    "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                    "Desc2=" + decoder["L_LONGMESSAGE0"];

                return false;
            }
        }


        /// <summary>
        /// HttpCall: The main method that is used for all API calls
        /// </summary>
        /// <param name="NvpRequest"></param>
        /// <returns></returns>
        public string HttpCall(string NvpRequest) //CallNvpServer
        {
            //To Add the credentials from the profile
            string strPost = NvpRequest + "&" + buildCredentialsNVPString();
            strPost = strPost + "&BUTTONSOURCE=" + HttpUtility.UrlEncode(BNCode);

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Provider.ApiEndPointUrl);
            objRequest.Timeout = Timeout;
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;

            try
            {
                using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
                {
                    myWriter.Write(strPost);
                }
            }
            catch (Exception e)
            {
                Log.WriteEntry(EntryType.Error, "Catalog", "PayPal HttpCall", exception: e);
            }

            //Retrieve the Response returned from the NVP API call to PayPal
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string result;
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Credentials added to the NVP string
        /// </summary>
        /// <returns></returns>
        private string buildCredentialsNVPString()
        {
            PayPalApiData codec = new PayPalApiData();

            if (!IsEmpty(Provider.ApiUsername))
                codec["USER"] = Provider.ApiUsername;

            if (!IsEmpty(Provider.ApiPassword))
                codec[PWD] = Provider.ApiPassword;

            if (!IsEmpty(Provider.ApiSignature))
                codec[SIGNATURE] = Provider.ApiSignature;

            if (!IsEmpty(Subject))
                codec["SUBJECT"] = Subject;

            codec["VERSION"] = "2.3";

            return codec.Encode();
        }

        /// <summary>
        /// Returns if a string is empty or null
        /// </summary>
        /// <param name="s">the string</param>
        /// <returns>true if the string is not null and is not empty or just whitespace</returns>
        public static bool IsEmpty(string s)
        {
            return s == null || s.Trim() == string.Empty;
        }
    }
}
