using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.CRM
{
    /// <summary>
    /// Indicates possible credention modes.
    /// </summary>
    public enum CredentialMode
    {
        /// <summary>
        /// Credential mode using Customer No and Customer Password.
        /// </summary>
        CustomerNo_CustomerPassword,
        /// <summary>
        /// Credential mode using Customer Username and Customer Password.
        /// </summary>
        CustomerUsername_CustomerPassword,
        /// <summary>
        /// Credential mode using Contact Email and Contact Password.
        /// </summary>
        ContactEmail_ContactPassword,
        /// <summary>
        /// Credential mode using Contact Username and Contact Password.
        /// </summary>
        ContactUsername_ContactPassword,
    }
}
