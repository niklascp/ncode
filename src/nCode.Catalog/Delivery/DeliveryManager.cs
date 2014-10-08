using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCode.Catalog.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace nCode.Catalog.Delivery
{
    public static class DeliveryManager
    {
        static DeliveryManager()
        {
            Initialize();
        }

        public static DeliveryProviderCollection Providers { get; private set; }

        private static void Initialize()
        {
            DeliverySection section = DeliverySection.Current;

            //Make sure that there is a custom section, and that the providers exist; if not setup, throw an error
            if (section == null)
                throw new ProviderException("The delivery section has not been setup correctly");

            //Instantiate the providers collection to store the collection with
            Providers = new DeliveryProviderCollection();

            //Instantiate the providers collection using the helper method defined in the framework
            ProvidersHelper.InstantiateProviders(section.Providers, Providers, typeof(DeliveryProvider));
        }
    }
}
