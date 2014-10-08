using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using nCode.Catalog.UI;

namespace nCode.Catalog
{
    /// <summary>
    /// Rewites links in content to SEO-friendly style.
    /// </summary>
    public class ItemRewriteHandler : ContentRewriteHandler
    {
        //Dictionary<Guid,string> urlCache;
        private readonly Regex regex;

        /// <summary>
        /// Creates a new ContentPageRewriteHandler.
        /// </summary>
        public ItemRewriteHandler()
        {
            regex = new Regex(@"(?<=<A.+HREF\s*=\s*[""'])(?:/[a-zA-Z]{2}-[a-zA-Z]{2})/Catalog/Item-View(?:\.aspx)\?ID=([^'"" >]*)(?=[ '""][^>]*>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private string GetSeoFrindlyUrl(Guid itemId)
        {
            using (var model = new CatalogModel())
            {
                var itemView = (from i in model.Items
                                from g in i.Localizations.Where(x => x.Culture == null)
                                from l in i.Localizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name).DefaultIfEmpty()
                                where i.ID == itemId
                                select new
                                {
                                    i.ItemNo,
                                    (l ?? g).Title
                                }).SingleOrDefault();

                if (itemView != null)
                {
                    return SeoUtilities.GetItemUrl(itemView.ItemNo, itemView.Title);
                }
            }

            return "/";
        }

        /// <summary>
        /// Gets the Regex used to match this handler against content.
        /// </summary>
        public override Regex Regex
        {
            get { return regex; }
        }

        /// <summary>
        /// Rewrites match in content before rendering.
        /// </summary>
        public override string RewritePreRenderMatch(Match match)
        {
            var itemId = new Guid(match.Groups[1].Value);
            return GetSeoFrindlyUrl(itemId);
        }
    }
}
