using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

using nCode;
using nCode.Configuration;

namespace nCode.Catalog
{
    public class ItemRouteHandler : IRouteHandler
    {
        private const string defaultPath = "/Catalog/Item-View.aspx";

        private string path;
        private GlobalizationSection globalizationSection;

        public ItemRouteHandler()
            : this(defaultPath)
        { }

        public ItemRouteHandler(string pagePath)
        {
            path = pagePath;
            globalizationSection = GlobalizationSection.GetSection();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string culture = requestContext.RouteData.Values["Culture"] as string;
            string itemNo = requestContext.RouteData.Values["ItemNo"] as string;

            if (itemNo.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
                itemNo = itemNo.Remove(itemNo.Length - 4);

            if (culture != null)
            {
                foreach (SupportedCulture supportedCulture in globalizationSection.SupportedCultures)
                {
                    if (string.Equals(supportedCulture.Name, culture, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(supportedCulture.Name);
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(supportedCulture.Name);
                        break;
                    }
                }
            }

            requestContext.HttpContext.Items["ItemNo"] = itemNo;

            return (Page)BuildManager.CreateInstanceFromVirtualPath(path, typeof(Page));
        }
    }
}
