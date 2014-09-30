using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace nCode.Configuration
{
    /// <summary>
    /// Represents the Admin Interface Section in the configuration. 
    /// The word "Interface" in this context is not well-choosen, since the section also 
    /// contains the information about Modules and ImportManagers.
    /// </summary>
    public class AdminInterfaceSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the Admin Interface Section.
        /// </summary>
        public static AdminInterfaceSection GetSection()
        {
            return ConfigurationManager.GetSection("nCode/adminInterface") as AdminInterfaceSection;
        }

        /// <summary>
        /// Gets a list of Module Settings. Use Settings.Modules to access the actual modules.
        /// </summary>
        [ConfigurationProperty("modules")]
        public ProviderSettingsCollection Modules
        {
            get { return (ProviderSettingsCollection)base["modules"]; }
        }

        /// <summary>
        /// Gets a list of Import Managers. Use Settings.ImportManagers to access the actual Import Managers.
        /// </summary>
        [ConfigurationProperty("importManagers")]
        public ImportManagerConfigurationCollection ImportManagers { get { return (ImportManagerConfigurationCollection)base["importManagers"]; } }

        /// <summary>
        /// Gets a list of shortcuts
        /// </summary>
        [ConfigurationProperty("shortcuts")]
        public ShortcutCollection Shortcuts
        {
            get
            {
                return this["shortcuts"] as ShortcutCollection;
            }
        }

        [ConfigurationProperty("supportInformation")]
        public SupportInformation SupportInformation
        {
            get
            {
                return this["supportInformation"] as SupportInformation;
            }
        }
    }
}
