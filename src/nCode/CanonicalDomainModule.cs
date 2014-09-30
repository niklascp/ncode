using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using nCode.Configuration;

namespace nCode
{
    /// <summary>
    /// Module for ensuring a single host header name is used for the website (SEO).
    /// </summary>
    public class CanonicalDomainModule : IHttpModule
    {
        const string currentHostMappingItem = "nCode.System.CurrentHostMapping";

        /// <summary>
        /// Gets the current host mapping, if any.
        /// </summary>
        public static HostMappingElement CurrentHostMapping
        {
            get
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException("Getting the CurrentHostMapping requires a current HttpContext.");
                return (HostMappingElement)HttpContext.Current.Items[currentHostMappingItem];
            }
            private set
            {
                if (HttpContext.Current == null)
                    throw new InvalidOperationException("Setting the CurrentHostMapping requires a current HttpContext.");
                HttpContext.Current.Items[currentHostMappingItem] = value;
            }
        }

        private void app_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            Uri requestUrl = app.Request.Url;
            
            SiteSection siteSection = SiteSection.GetSection();
            if (siteSection != null && siteSection.HostMappings != null)
            {
                /* 2011-07-29: This site uses the new host mapping functionality. */
                var mapping = siteSection.HostMappings.FirstOrDefault(x => x.Name.Equals(requestUrl.Host, StringComparison.InvariantCultureIgnoreCase));
                if (mapping != null)
                {
                    CurrentHostMapping = mapping;

                    /* We are only going to redirect the root request. */
                    if (requestUrl.PathAndQuery == "/")
                        app.Response.RedirectPermanent(mapping.FrontpagePath);

                    return;
                }
            }

            if (!Settings.EnforceCanonicalDomain || string.IsNullOrEmpty(Settings.Url))
                return;

            Uri canonicalUrl = new Uri(Settings.Url);
            /*
                * 2011-04-13: Fix: Never redirect from localhost. 
                */
            if (string.Equals(requestUrl.Host, "localhost", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!string.Equals(requestUrl.Host, canonicalUrl.Host, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(requestUrl.Scheme, canonicalUrl.Scheme, StringComparison.InvariantCultureIgnoreCase))
                app.Response.RedirectPermanent(Settings.Url.TrimEnd(new char[] { '/' }) + requestUrl.PathAndQuery);
        }

        public void Init(HttpApplication app)
        {
            /*
             * Hook up evetns. 
             */
            app.BeginRequest += new EventHandler(app_BeginRequest);
        }

        public void Dispose()
        {

        }
    }
}
