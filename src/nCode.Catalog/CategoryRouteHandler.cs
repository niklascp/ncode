using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

using nCode.Configuration;

namespace nCode.Catalog
{
    public class CategoryRouteHandler : IRouteHandler
    {
        private const string defaultPath = "/Catalog/Item-List.aspx";

        private string path;
        private GlobalizationSection globalizationSection;

        public CategoryRouteHandler()
            : this(defaultPath)
        { }

        public CategoryRouteHandler(string pagePath)
        {
            path = pagePath;
            globalizationSection = GlobalizationSection.GetSection();
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string culture = requestContext.RouteData.Values["Culture"] as string;
            string categoryNo = requestContext.RouteData.Values["CategoryNo"] as string;

            if (categoryNo.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase))
                categoryNo = categoryNo.Remove(categoryNo.Length - 4);

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

            return (Page)BuildManager.CreateInstanceFromVirtualPath(path, typeof(Page));
        }
    }
}
