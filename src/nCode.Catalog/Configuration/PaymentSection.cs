using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;

namespace nCode.Catalog.Configuration
{
    public class PaymentSection : ConfigurationSection
    {
        private const string sectionPath = "nCode.catalog/payment";

        public static PaymentSection Current
        {
            get
            {
                return (PaymentSection)ConfigurationManager.GetSection(sectionPath);
            }
        }

        [ConfigurationProperty("providers", IsRequired = false)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }
    }
}
