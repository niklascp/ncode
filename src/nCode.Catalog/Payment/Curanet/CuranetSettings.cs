using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace nCode.Catalog.Payment.Curanet
{
    [Obsolete("Use properties on the CuranetPaymentProvider instance.")]
    public static class CuranetSettings
    {
        static string ns = "nCode.Catalog.Payment.Curanet";

        public static string ShopID
        {
            get
            {
                return ConfigurationManager.AppSettings[ns + ".ShopID"];
            }
        }

        /*
        public static string Username
        {
            get
            {
                return ConfigurationManager.AppSettings[ns + ".Username"];
            }
        }

        public static string Password
        {
            get
            {
                return ConfigurationManager.AppSettings[ns + ".Password"];
            }
        }
        */

        public static string ProxyUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[ns + ".ProxyUrl"];
            }
        }

        /*
        public static string ApiUrl {
            get
            {
                return ConfigurationManager.AppSettings[ns + ".ApiUrl"];
            }
        }
        */
    }
}
