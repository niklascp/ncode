using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace nCode.CRM.UI
{
    /// <summary>
    /// Models a Crm Authention protected page.
    /// </summary>
    public class ProtectedPage : nCode.UI.Page
    {
        /// <summary>
        /// Overrides so that authention is handled.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            if (!CrmAuthentication.Authenticated)
                Response.Redirect(string.Format("{0}/crm/Login", CultureUrlPrefix));

            base.OnPreInit(e);
        }
    }
}
