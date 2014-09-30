using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    public class GlobalizationSection : ConfigurationSection
    {
        public static GlobalizationSection GetSection()
        {
            return ConfigurationManager.GetSection("nCode/globalization") as GlobalizationSection;
        }

        [ConfigurationProperty("supportedCultures")]
        public SupportedCultureCollection SupportedCultures
        {
            get
            {
                return this["supportedCultures"] as SupportedCultureCollection;
            }
        }
    }
}
