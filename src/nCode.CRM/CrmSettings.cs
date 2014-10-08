using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.CRM
{
    /// <summary>
    /// Crm Settings
    /// </summary>
    public static class CrmSettings
    {
        /// <summary>
        /// Gets or sets the current Credential Mode (site specific).
        /// </summary>
        public static CredentialMode CredentialMode
        {
            get { return Settings.GetProperty<CredentialMode>("nCode.CRM.CredentialMode", CredentialMode.ContactEmail_ContactPassword); }
            set { Settings.SetProperty<CredentialMode>("nCode.CRM.CredentialMode", value); }
        }
    }
}
