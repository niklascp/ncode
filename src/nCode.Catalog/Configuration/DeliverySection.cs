using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;

namespace nCode.Catalog.Configuration
{
    /// <summary>
    /// Represents the Delivery Configuration Section.
    /// </summary>
    public class DeliverySection : ConfigurationSection
    {
        private const string sectionPath = "nCode.catalog/delivery";

        /// <summary>
        /// Gets the Current Configuration.
        /// </summary>
        public static DeliverySection Current
        {
            get
            {
                return (DeliverySection)ConfigurationManager.GetSection(sectionPath);
            }
        }

        /// <summary>
        /// Gets a list of Confiugred Delivery Providers.
        /// </summary>
        [ConfigurationProperty("providers", IsRequired = false)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base["providers"]; }
        }
    }
}
