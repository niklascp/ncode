using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Configuration;

using nCode.Catalog.Configuration;

namespace nCode.Catalog.Payment
{
    public static class PaymentManager
    {
        static PaymentManager()
        {
            Initialize();
        }

        public static PaymentProviderCollection Providers { get; private set; }

        private static void Initialize()
        {
            PaymentSection section = PaymentSection.Current;

            //Make sure that there is a custom section, and that the providers exist; if not setup, throw an error
            if (section == null)
                throw new ProviderException("The payment section has not been setup correctly.");

            //Instantiate the providers collection to store the collection with
            Providers = new PaymentProviderCollection();

            //Instantiate the providers collection using the helper method defined in the framework
            ProvidersHelper.InstantiateProviders(section.Providers, Providers, typeof(PaymentProvider));
        }
    }
}
