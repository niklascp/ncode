using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.CRM
{
    /// <summary>
    /// Provides access to the Crm Authentication Control
    /// </summary>
    public class CrmAuthentication
    {
        private const string sessionKey = "nCode.CRM.Identity";
        private const string customerIdSessionKey = "nCode.CRM.Identity.CustomerID";
        private const string contactNoSessionKey = "nCode.CRM.Identity.ContactID";

        /// <summary>
        /// Gets the identity of the current user.
        /// </summary>
        public static string Identity
        {
            get { return (string)HttpContext.Current.Session[sessionKey]; }
        }

        /// <summary>
        /// True if the current user is authenticated.
        /// </summary>
        public static bool Authenticated
        {
            get { return HttpContext.Current.Session[sessionKey] != null; }
        }

        /// <summary>
        /// Signs in with the given identity.
        /// </summary>
        /// <param name="identity"></param>
        public static void SignIn(string identity)
        {
            using (var context = new CrmDbContext())
            {
                Customer customer = null;
                CustomerContact contact = null;

                switch (CrmSettings.CredentialMode)
                {
                    case CredentialMode.ContactEmail_ContactPassword:
                        contact = context.CustomerContacts.SingleOrDefault(c => c.Email == identity);
                        customer = contact.Customer;
                        break;
                    case CredentialMode.CustomerNo_CustomerPassword:
                        customer = context.Customers.SingleOrDefault(c => c.CustomerNo == identity);
                        break;
                    case CredentialMode.CustomerUsername_CustomerPassword:
                        customer = context.Customers.SingleOrDefault(c => c.Username == identity);
                        break;
                }

                HttpContext.Current.Session[sessionKey] = identity;

                if (customer != null)
                    HttpContext.Current.Session[customerIdSessionKey] = customer.ID;
                else
                    HttpContext.Current.Session[customerIdSessionKey] = null;

                if (contact != null)
                    HttpContext.Current.Session[contactNoSessionKey] = contact.ID;
                else
                    HttpContext.Current.Session[contactNoSessionKey] = null;
            }
        }

        /// <summary>
        /// Signs out.
        /// </summary>
        public static void SignOut()
        {
            HttpContext.Current.Session[sessionKey] = null;
            HttpContext.Current.Session[customerIdSessionKey] = null;
            HttpContext.Current.Session[contactNoSessionKey] = null;
        }

        /// <summary>
        /// Gets the Customer of the current user.
        /// </summary>
        public static Customer Customer
        {
            get
            {
                var customerId = (Guid?)HttpContext.Current.Session[customerIdSessionKey];
                
                if (customerId == null)
                    return null;

                using (var context = new CrmDbContext())
                {
                    return context.Customers.Find(customerId);
                }
            }
        }

        /// <summary>
        /// Gets the Customer Contact (if available) of the current user.
        /// </summary>
        public static CustomerContact Contact
        {
            get
            {
                var contactId = (Guid?)HttpContext.Current.Session[contactNoSessionKey];

                if (contactId == null)
                    return null;

                using (var context = new CrmDbContext())
                {
                    return context.CustomerContacts.Find(contactId);
                }
            }
        }

        /// <summary>
        /// Returns true if authentication is successfull with the given identity and password. 
        /// The authentication relies on the current Credential Mode <see cref="CrmSettings.CredentialMode"/>
        /// </summary>
        public static bool Authenticate(string identity, string password)
        {
            using (var context = new CrmDbContext())
            {
                switch (CrmSettings.CredentialMode)
                {
                    case CredentialMode.ContactEmail_ContactPassword:
                        var contact = context.CustomerContacts.SingleOrDefault(c => c.Email == identity);
                        if (contact != null && contact.Password == password)
                            return true;
                        return false;
                    case CredentialMode.CustomerNo_CustomerPassword:
                        var customer = context.Customers.SingleOrDefault(c => c.CustomerNo == identity);
                        if (customer != null && customer.Password == password)
                            return true;
                        return false;
                    case CredentialMode.CustomerUsername_CustomerPassword:
                        customer = context.Customers.SingleOrDefault(c => c.Username == identity);
                        if (customer != null && customer.Password == password)
                            return true;
                        return false;
                    default:
                        return false;
                }
            }
        }
    }
}
