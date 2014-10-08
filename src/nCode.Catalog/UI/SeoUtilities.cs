using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace nCode.Catalog.UI
{
    public static class SeoUtilities
    {
        static Regex matchNonLatin = new Regex(@"[^A-Za-z0-9]", RegexOptions.Compiled);
        static Regex matchMultipleDashes = new Regex(@"-+", RegexOptions.Compiled);

        static SeoUtilities() {

        }

        public static bool UseUrlRouting
        {
            get
            {
                return Settings.GetProperty<bool>("nCode.Catalog.UrlRouting", false);
            }
            set
            {
                Settings.GetProperty<bool>("nCode.Catalog.UrlRouting", value);
            }
        }

        public static string GetSafeTitle(string title)
        {
            string safeTitle = (title ?? string.Empty).Trim();

            if (string.Equals(Thread.CurrentThread.CurrentUICulture.Name, "da-DK", StringComparison.InvariantCultureIgnoreCase))
            {
                safeTitle = safeTitle.Replace("Æ", "Ae");
                safeTitle = safeTitle.Replace("Ø", "Oe");
                safeTitle = safeTitle.Replace("Å", "Aa");
                safeTitle = safeTitle.Replace("æ", "ae");
                safeTitle = safeTitle.Replace("ø", "oe");
                safeTitle = safeTitle.Replace("å", "aa");
            }

            safeTitle = matchNonLatin.Replace(safeTitle, "-");
            safeTitle = matchMultipleDashes.Replace(safeTitle, "-");
            safeTitle = safeTitle.Trim(new char[] { '-' });

            if (string.IsNullOrEmpty(safeTitle))
                safeTitle = "Item";

            return safeTitle;
        }

        public static string GetItemUrl(string itemNo, string itemTitle, string routeName = "Catalog.Item")
        {
            var routeValues = new RouteValueDictionary();
            
            routeValues.Add("ItemNo", itemNo);

            VirtualPathData vpd;

            if (UseUrlRouting)
            {
                routeValues.Add("ItemTitle", GetSafeTitle(itemTitle));
            }
            else
            {
                routeName = "System.Page";
                routeValues.Add("Path", "catalog/Item-View");
            }

            if (Settings.SupportedCultureNames.Any() && string.Equals(Settings.SupportedCultureNames.First(), CultureInfo.CurrentUICulture.Name)) {
                routeName += "(DefaultCulture)";
            }
            else {
                routeName += "(SpecificCulture)";
                routeValues.Add("Culture", CultureInfo.CurrentUICulture.Name.ToLower());
            }

            vpd = RouteTable.Routes.GetVirtualPath(null, routeName, routeValues);

            return vpd.VirtualPath;
        }
    }
}
