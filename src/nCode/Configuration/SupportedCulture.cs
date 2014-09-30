using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Resources;

namespace nCode.Configuration
{
    public class SupportedCulture : ConfigurationElement
    {
        private static ResourceManager resourceMan;
        public static ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new ResourceManager("nCode.Resources.SupportedCulture", typeof(SupportedCulture).Assembly);
                }
                return resourceMan;
            }
        }

        private string resourceCode;
        private CultureInfo culture;

        public SupportedCulture()
        { }

        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            resourceCode = Regex.Replace(Name, "-", "_");
            culture = CultureInfo.GetCultureInfo(Name);
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        public string DisplayName
        {
            get
            {
                string displayName = ResourceManager.GetString(resourceCode);
                if (displayName == null)
                    displayName = Name;
                return displayName;
            }
        }

        public string NativeName
        {
            get
            {
                string nativeName = ResourceManager.GetString(resourceCode, culture);
                if (nativeName == null)
                    nativeName = Name;
                return nativeName;
            }
        }
    }
}