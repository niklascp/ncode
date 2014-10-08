using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;

namespace nCode.CMS
{
    /// <summary>
    /// Checkes the Culture and the StaticPath parameter, and returns true if a corresponding content page exists.
    /// </summary>
    public class ContentPageRouteContraint : IRouteConstraint 
    {
        private static Regex legacyPathRecognizer = new Regex(@"cms/[a-z]{2}-[a-z]{2}/.*");

        /// <summary>
        /// Checkes the Culture and the StaticPath parameter, and returns true if a corresponding content page exists.
        /// </summary>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            /* We don not contraint legacy paths, as they should not conflict with other routes. */
            if (CmsSettings.UseLegacyPathSchema || CmsSettings.AcceptLegacyPathSchema && legacyPathRecognizer.IsMatch(httpContext.Request.Url.PathAndQuery))
                return true;

            string staticPath = values["StaticPath"] as string;

            if (staticPath == null)
                return false;

            if (staticPath.StartsWith("cms/"))
                staticPath = staticPath.Substring(4);

            /* Remove any extension */
            int pos = staticPath.LastIndexOf(".");

            /* We did find a dot - remove it */
            if (pos >= 0)
                staticPath = staticPath.Substring(0, pos);

            if (staticPath.Length == 0)
                return false;

            return CmsPathMappingCache.PathMapping != null && CmsPathMappingCache.PathMapping.ContainsKey(staticPath.ToLowerInvariant());
        }
    }
}
