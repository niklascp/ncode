using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Threading;
using System.Globalization;
using System.Web.Security;
using nCode.CMS.UI;
using System.Web.UI;
using System.IO;

namespace nCode.CMS
{
    /// <summary>
    /// Rewrites Url in the format /cms/(language code)/(static path).aspx to the correct cms page.
    /// </summary>
    public class RewriteModule : IHttpModule
    {
        const string virtualUrlItem = "nCode.CMS.RewriteModule.VirtualUrl";
        const string cachedPathAfterRewriteItem = "nCode.CMS.RewriteModule.CachedPathAfterRewrite";
        const string clientQueryStringItem = "nCode.CMS.RewriteModule.ClientQueryStringItem";
        const string prefix = "/cms/";
        
        public void Init(HttpApplication context)
        {
            // WARNING!  This does not work with Windows authentication!
            // If you are using Windows authentication, 
            // change to app.BeginRequest
            context.AuthorizeRequest += new EventHandler(RewriteModule_AuthorizeRequest);

            // Add exstra event for handling forms currection
            context.PreRequestHandlerExecute += new EventHandler(RewriteModule_PreRequestHandlerExecute);
            context.PostRequestHandlerExecute += new EventHandler(RewriteModule_PostRequestHandlerExecute);
        }

        private void RewriteModule_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;

            // Don't rewrite files that actually exists.
            if (File.Exists(app.Request.PhysicalPath))
                return;

            // Is this a CMS Page request?
            if (app.Request.Path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                string requestUrl = app.Request.RawUrl;
                string urlForRewrite = requestUrl;
                string requestQuerystring = string.Empty;
                int pos = requestUrl.IndexOf('?');
                if (pos >= 0)
                {
                    requestQuerystring = requestUrl.Substring(pos + 1);
                    urlForRewrite = requestUrl.Substring(0, pos);
                }

                // Save the original query string.
                app.Context.Items[clientQueryStringItem] = requestQuerystring;
                // Save the original request url.
                app.Context.Items[virtualUrlItem] = requestUrl;


                // Remove the prefix.
                string cmsPath = app.Request.Path.Substring(prefix.Length);

                // Split the request path into language code and cms path.
                string[] cmsParts = cmsPath.Split(new char[] { '/' }, 2);

                if (cmsParts.Length != 2)
                    throw new ApplicationException("Missing or invalid path to indicate CMS Page.");

                // Remove the file exstension
                pos = cmsPath.LastIndexOf(".");
                if (pos >= 0)
                {
                    cmsPath = cmsPath.Substring(0, pos);
                }

                // Set Culture
                CultureInfo culture = new CultureInfo(cmsParts[0]);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                // Set Language Cookie
                app.Response.Cookies.Add(new HttpCookie("Language", culture.Name));

                // Load the Content Object from the rest of the path.
                ContentPageCollection contents = new ContentPageCollection();
                contents.LoadContent();
                ContentPage content = contents[cmsPath];

                if (content == null)
                    throw new ApplicationException("The requested CMS Page does not exits.");

                if (content.IsProtected && !content.PrivilegeContext.HasPrivilege("View"))
                    app.Response.Redirect(FormsAuthentication.LoginUrl + "?returnurl=" + HttpUtility.UrlEncode(content.Url));

                ContentPage.Current = content;

                // LinkUrl?
                if (content.LinkUrl != null)
                {
                    if (content.LinkMode == LinkMode.Normal)
                    {
                        app.Response.Redirect(content.LinkUrl);
                    }
                    else if (content.LinkMode == LinkMode.UrlRewrite)
                    {
                        app.Context.RewritePath(content.LinkUrl, false);
                    }
                    else
                    {
                        throw new ApplicationException("The requested CMS Page uses an unsupported Link Mode.");
                    }
                }
                else
                {
                    app.Response.StatusCode = 200;
                    app.Context.RewritePath("~/CMS/Content-View.aspx?Path=" + cmsPath, false);
                }
            }
        }

        private void RewriteModule_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            // Add event handler for Pages
            HttpApplication app = sender as HttpApplication;
            System.Web.UI.Page page = app.Context.CurrentHandler as System.Web.UI.Page;
            if (page != null)
            {
                page.PreInit += new EventHandler(Page_PreInit);
            }
        }

        private void Page_PreInit(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            string virtualUrl = (string)context.Items[virtualUrlItem];

            if (!string.IsNullOrEmpty(virtualUrl))
            {
                int pos = virtualUrl.IndexOf('?');
                if (pos >= 0)
                {
                    virtualUrl = virtualUrl.Substring(0, pos);
                }

                string fullQueryString = context.Request.QueryString.ToString();

                // Save the page's actual request path.
                context.Items[cachedPathAfterRewriteItem] = context.Request.Path + "?" + fullQueryString;

                // Get client query string.
                string clientQueryString = (string)context.Items[clientQueryStringItem];
                context.RewritePath(virtualUrl, string.Empty, clientQueryString, true);
                Page page = sender as Page;
                clientQueryString = page.ClientQueryString;

                // Rewrite to virtual path.
                context.RewritePath(virtualUrl, string.Empty, clientQueryString, true);
            }
        }

        private void RewriteModule_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;

            // Ensure that the path is re-re-written back to the actual request path.
            string cachedPath = (string)app.Context.Items[cachedPathAfterRewriteItem];
            if (!string.IsNullOrEmpty(cachedPath))
            {
                app.Context.RewritePath(cachedPath);
            }
        }

        public void Dispose()
        {
        }
    }
}
