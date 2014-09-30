using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace nCode.Configuration
{
    public class SupportInformation : ConfigurationElement
    {
        [ConfigurationProperty("contactDataUrl")]
        public string ContactDataUrl
        {
            get { return (string)this["contactDataUrl"]; }
        }

        [ConfigurationProperty("feedUrl")]
        public string FeedUrl
        {
            get { return (string)this["feedUrl"]; }
        }
    }
}
