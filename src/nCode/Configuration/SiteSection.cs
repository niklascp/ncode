using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    /// <summary>
    /// Represents the Site Section in the configuration. 
    /// </summary>
    public class SiteSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the Admin Interface Section.
        /// </summary>
        public static SiteSection GetSection()
        {
            return ConfigurationManager.GetSection("nCode/site") as SiteSection;
        }

        /// <summary>
        /// Gets a list of Module Settings. Use Settings.Modules to access the actual modules.
        /// </summary>
        [ConfigurationProperty("hostMappings")]
        public HostMappingCollection HostMappings
        {
            get { return (HostMappingCollection)base["hostMappings"]; }
        }
    }
}
