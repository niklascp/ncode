using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace nCode.Configuration
{
    public class Shortcut : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
        }

        [ConfigurationProperty("icon")]
        public string Icon
        {
            get { return (string)this["icon"]; }
        }

        [ConfigurationProperty("target")]
        public string Target
        {
            get { return (string)this["target"]; }
        }
    }
}
