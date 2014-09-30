using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using nCode.Configuration;
using System.Web.Compilation;
using System.Web.UI;
using System.Threading;
using System.Globalization;

namespace nCode
{
    /// <summary>
    /// Routes CMS page requests to Content-View.aspx
    /// </summary>
    public class CultureRouteHandler : IRouteHandler
    {
        GlobalizationSection globalizationSection;

        public CultureRouteHandler()
        {
            globalizationSection = GlobalizationSection.GetSection();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string culture = requestContext.RouteData.Values["Culture"] as string;
            string path = requestContext.RouteData.Values["Path"] as string;

            if (string.IsNullOrEmpty(path))
                path = "Default.aspx";
            /* Allow us to strip the .aspx, and request pages til /da-dk/catalog/Basket */
            else if (!path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                path += ".aspx";            

            if (culture != null)
            {
                /* The route contraint has already validated that this is sane. */
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            }

            return (Page)BuildManager.CreateInstanceFromVirtualPath(string.Format("~/{0}", path), typeof(Page));
        }
    }
}
