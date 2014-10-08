using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCode.UI;
using System.Globalization;

namespace nCode.CMS.UI
{
    public static class CmsNavigationExtensions
    {
        public static List<ContentPageNode> GetMenu(this Navigation n, string menu, string language = null, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            if (language == null)
                language = CultureInfo.CurrentUICulture.Name;

            if (viewFilter == null)
                viewFilter = x => x.IsVisible;

            return NavigationUtilities.GetMenu(menu, language, -1, viewFilter, traverseFilter);
        }

        public static ContentPageNodeView GetMenuView(this Navigation n, string menu, string language = null, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null) 
        {
            if (language == null)
                language = CultureInfo.CurrentUICulture.Name;

            if (viewFilter == null)
                viewFilter = x => x.IsVisible;

            return NavigationUtilities.GetMenuView(menu, language, -1, viewFilter, traverseFilter);
        }

        public static List<ContentPageNode> GetSubmenu(this Navigation n, ContentPage parent, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            if (viewFilter == null)
                viewFilter = x => x.IsVisible;

            return NavigationUtilities.GetSubmenu(parent, -1, viewFilter, traverseFilter);
        }

        public static ContentPageNodeView GetSubmenuView(this Navigation n, ContentPage parent, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            if (viewFilter == null)
                viewFilter = x => x.IsVisible;

            return NavigationUtilities.GetSubmenuView(parent, -1, viewFilter, traverseFilter);
        }
    }
}
