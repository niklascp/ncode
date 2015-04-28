using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using nCode.Catalog.Models;
using System.Web.UI;
using nCode.Catalog.ViewModels;
using System.Web.UI.HtmlControls;
using nCode.UI;

namespace nCode.Catalog.UI
{
    public static class SeoUtilities
    {
        static Regex matchNonLatin = new Regex(@"[^A-Za-z0-9]", RegexOptions.Compiled);
        static Regex matchMultipleDashes = new Regex(@"-+", RegexOptions.Compiled);

        static SeoUtilities()
        {

        }

        public static ItemViewRequest ResolveItemFromHttpContext(HttpContext context, string routeName = "Catalog.Item")
        {
            var itemViewRequest = new ItemViewRequest();
            itemViewRequest.OriginalPath = context.Request.Url.PathAndQuery;

            using (var catalogContext = new CatalogDbContext())
            {
                /* If ItemNo is available in QueryString, or has been added to the Items Collection. */
                if (!string.IsNullOrEmpty(context.Request.QueryString["ItemNo"]) || !string.IsNullOrEmpty(context.Items["ItemNo"] as string))
                {
                    var itemNo = !string.IsNullOrEmpty(context.Request.QueryString["ItemNo"]) ? context.Request.QueryString["ItemNo"] : (string)HttpContext.Current.Items["ItemNo"];

                    var itemView = (
                        from i in catalogContext.Items.Where(x => x.ItemNo == itemNo)
                        from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from g in i.Localizations.Where(x => x.Culture == null)
                        select new
                        {
                            i.ItemNo,
                            (l ?? g).Title
                        }
                    ).SingleOrDefault();

                    if (itemView != null)
                    {
                        itemViewRequest.ItemNo = itemView.ItemNo;
                        itemViewRequest.CanonicalPath = GetItemUrl(itemView.ItemNo, itemView.Title, routeName);
                        return itemViewRequest;
                    }
                }
                else if (!string.IsNullOrEmpty(context.Request.QueryString["ID"]))
                {
                    var id = new Guid(context.Request.QueryString["ID"]);

                    var itemView = (
                        from i in catalogContext.Items.Where(x => x.ID == id)
                        from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                        from g in i.Localizations.Where(x => x.Culture == null)
                        select new
                        {
                            i.ItemNo,
                            (l ?? g).Title
                        }
                    ).SingleOrDefault();

                    if (itemView != null)
                    {
                        itemViewRequest.ItemNo = itemView.ItemNo;
                        itemViewRequest.CanonicalPath = GetItemUrl(itemView.ItemNo, itemView.Title, routeName);
                        return itemViewRequest;
                    }
                }                
            }

            /* Unable to resolve Item. */
            return null;
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

            if (Settings.SupportedCultureNames.Any() && string.Equals(Settings.SupportedCultureNames.First(), CultureInfo.CurrentUICulture.Name))
            {
                routeName += "(DefaultCulture)";
            }
            else
            {
                routeName += "(SpecificCulture)";
                routeValues.Add("Culture", CultureInfo.CurrentUICulture.Name.ToLower());
            }

            vpd = RouteTable.Routes.GetVirtualPath(null, routeName, routeValues);

            return vpd.VirtualPath;
        }
    }
}
