using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    public class HostMappingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("frontpagePath", IsRequired = true)]
        public string FrontpagePath
        {
            get { return (string)this["frontpagePath"]; }
        }

        [ConfigurationProperty("masterPageFile", IsRequired = false)]
        public string MasterPageFile
        {
            get { return (string)this["masterPageFile"]; }
        }

        [ConfigurationProperty("defaultCulture", IsRequired = false)]
        public string DefaultCulture
        {
            get { return (string)this["defaultCulture"]; }
        }
    }
}
