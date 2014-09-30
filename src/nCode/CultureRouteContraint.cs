using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Globalization;
using System.IO;

namespace nCode
{
    /// <summary>
    /// Checkes the Culture and the StaticPath parameter, and returns true if a corresponding content page exists.
    /// </summary>
    public class CultureRouteContraint : IRouteConstraint 
    {
        /// <summary>
        /// Checkes the Culture and the Path parameter, and returns true if a corresponding Web Forms page exists.
        /// </summary>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var culture = values["Culture"] as string;
            var path = values["Path"] as string;

            if (string.IsNullOrEmpty(path))
                path = "Default.aspx";
            /* Allow us to strip the .aspx, and request pages til /da-dk/catalog/Basket */
            else if (!path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                path += ".aspx";

            /* The supplied cultures is not supported by this installation */
            if (culture != null && !Settings.SupportedCultureNames.Any(x => x.Equals(culture, StringComparison.OrdinalIgnoreCase)))
                return false;                

            return File.Exists(httpContext.Server.MapPath(string.Format("~/{0}", path)));
        }
    }
}
