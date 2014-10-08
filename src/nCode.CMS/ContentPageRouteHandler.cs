using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;
using System.Globalization;
using System.Threading;
using System.Web.Security;
using System.Linq;
using System.Data.Linq;
using System.Text.RegularExpressions;

namespace nCode.CMS
{
    public class ContentPageRouteHandler : IRouteHandler
    {
        private static Regex legacyPathRecognizer = new Regex(@"cms/[a-z]{2}-[a-z]{2}/.*");

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            bool redirect = false;
            string staticPath = requestContext.RouteData.Values["StaticPath"] as string;

            if (staticPath.StartsWith("cms/"))
            {
                staticPath = staticPath.Substring(4);
                redirect = true;
            }

            // Remove any extension
            int pos = staticPath.LastIndexOf(".");

            /* We did find a dot - remove it */
            if (pos >= 0)
            {
                staticPath = staticPath.Substring(0, pos);

                if (CmsSettings.OmitExtension)
                    redirect = true;
            }

            // Load the Content Object from the rest of the path.
            var id = CmsPathMappingCache.PathMapping[staticPath.ToLowerInvariant()];
            ContentPage content = Utilities.GetContentPage(id);

            if (content == null)
                throw new HttpException(404, "The requested CMS Content Page '" + staticPath + "' was not found.");

            // Set Culture
            CultureInfo culture = new CultureInfo(content.Language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            /* SEO - Redirect if we are omitting extension, and this request contained one. (20100419) */
            if (redirect) 
            {
                requestContext.HttpContext.Response.RedirectPermanent(content.Url + requestContext.HttpContext.Request.Url.Query);
                return new Page();
            }
            
            nCode.UI.Navigation.Current = content;

            // LinkUrl?
            if (content.LinkUrl != null)
            {
                if (content.LinkMode == LinkMode.Normal)
                {
                    requestContext.HttpContext.Response.RedirectPermanent(content.LinkUrl);
                    return new Page();
                }
                else if (content.LinkMode == LinkMode.UrlRewrite)
                {
                    return BuildManager.CreateInstanceFromVirtualPath(content.LinkUrl, typeof(Page)) as Page;
                }
                else
                {
                    throw new ApplicationException("The requested CMS Page uses an unsupported Link Mode.");
                }
            }
            else
            {
                return BuildManager.CreateInstanceFromVirtualPath("~/CMS/Content-View.aspx", typeof(Page)) as Page;
            }
        } 
    }
}
