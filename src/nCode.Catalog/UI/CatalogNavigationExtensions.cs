/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCode.UI;
using System.Globalization;

namespace nCode.Catalog.UI
{
    public static class CatalogNavigationExtensions
    {
        public static IList<CategoryInfoNode> GetCategoryInfo(this Navigation n, string language = null, CategoryInfoNodeFilter viewFilter = null, CategoryInfoNodeFilter traverseFilter = null)
        {
            if (language == null)
                language = CultureInfo.CurrentUICulture.Name;

            if (viewFilter == null)
                viewFilter = x => x.IsVisible;

            return CategoryUtilities.GetCategoryTree(language, viewFilter, traverseFilter);
        }

        public static CategoryInfoView GetCategoryInfoView(this Navigation n, string language = null, CategoryInfoNodeFilter viewFilter = null, CategoryInfoNodeFilter traverseFilter = null) 
        {
            return new CategoryInfoView(n.GetCategoryInfo(language, viewFilter, traverseFilter));
        }
    }
}
