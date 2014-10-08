/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

namespace nCode.CMS.UI
{
    public static class NavigationUtilities
    {
        private static HashSet<string> cacheKeys;

        static NavigationUtilities()
        {
            cacheKeys = new HashSet<string>();
        }

        private static List<ContentPageNode> TraverseContentPage(SqlConnection conn, string culture, Guid? parentID, int depth, int maxDepth, Func<ContentPageNode, bool> viewFilter, Func<ContentPageNode, bool> traverseFilter)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT " + ContentPageFactory.SelectFields + " FROM CMS_ContentPage");

            if (parentID.HasValue)
            {
                sb.AppendLine("WHERE Language = @Language AND ParentID = @Parent");
                cmd.Parameters.AddWithValue("@Language", culture);
                cmd.Parameters.AddWithValue("@Parent", parentID);
            }
            else
            {
                sb.AppendLine("WHERE Language IS NULL AND ParentID IS NULL");
            }

            sb.AppendLine("ORDER BY [DisplayIndex]");

            cmd.CommandText = sb.ToString();

            /* Load Content Pages */
            SqlDataReader rdr = cmd.ExecuteReader();

            var contentPages = new List<ContentPageNode>();

            while (rdr.Read())
            {
                var contentPage = ContentPageFactory.CreateContentPageNode(rdr);
                contentPage.Depth = depth;

                if (viewFilter == null || viewFilter(contentPage))
                {
                    contentPages.Add(contentPage);
                }
            }

            rdr.Close();

            /* Traverse Content Pages */
            if (maxDepth == -1 || depth < maxDepth)
            {
                foreach (var contentPage in contentPages)
                {
                    if (traverseFilter == null || traverseFilter(contentPage))
                    {
                        /* Pre traverse children */
                        if (contentPage.ContentType != null)
                            contentPage.ContentType.PreTraverseChildren(contentPage);

                        /* Traverse children */
                        var subContentPages = TraverseContentPage(conn, culture, contentPage.ID, depth + 1, maxDepth, viewFilter, traverseFilter);

                        foreach (var subContentPage in subContentPages)
                            subContentPage.Parent = contentPage;

                        /* Post traverse children */
                        if (contentPage.ContentType != null)
                            contentPage.ContentType.PostTraverseChildren(contentPage);
                    }
                }
            }

            return contentPages;
        }

        private static void BuildPathInternal(ContentPage contentPage, List<ContentPage> path)
        {
            if (contentPage == null)
                return;

            /* Check that this is not a Menu pseudo Content Page. */
            if (contentPage.ParentId != null)
            {
                var parentPage = Utilities.GetContentPage(contentPage.ParentId.Value);
                BuildPathInternal(parentPage, path);

                path.Add(contentPage);
            }
        }

        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static List<ContentPageNode> GetRoot(string language, int maxDepth = -1, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                return TraverseContentPage(conn, language, null, 0, maxDepth, viewFilter, traverseFilter);
            }
        }

        /* Menu */

        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static List<ContentPageNode> GetMenu(string menu, string language, int maxDepth = -1, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            string cacheKey = null;

            /* Check Cache */
            if (CmsSettings.CacheMenuViews && HttpContext.Current != null)
            {
                var cacheKeyObject = new { menu, language, maxDepth, viewFilter, traverseFilter };
                cacheKey = "nCode.CMS.Menu(" + ((uint)cacheKeyObject.GetHashCode()).ToString() + ")";

                if (HttpContext.Current.Cache[cacheKey] != null)
                {
                    var menuItems = (List<ContentPageNode>)HttpContext.Current.Cache[cacheKey];
                    return menuItems;
                }
            }

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                var getMenuIdCommand = new SqlCommand("SELECT ID FROM CMS_ContentPage WHERE Language IS NULL AND Title = @Menu", conn);
                getMenuIdCommand.Parameters.AddWithValue("@Menu", menu);

                var menuId = (Guid?)getMenuIdCommand.ExecuteScalar();

                if (menuId == null)
                    throw new ApplicationException(String.Format("CMS Menu '{0}' was not found.", menu));

                var menuItems = TraverseContentPage(conn, language, menuId, 0, maxDepth, viewFilter, traverseFilter);

                if (CmsSettings.CacheMenuViews && HttpContext.Current != null)
                {
                    lock (cacheKeys)
                    {
                        cacheKeys.Add(cacheKey);
                    }

                    HttpContext.Current.Cache.Add(cacheKey, menuItems, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Default, null);
                }

                return menuItems;
            }
        }

        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static ContentPageNodeView GetMenuView(string menu, string language, int maxDepth = -1, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            return new ContentPageNodeView(GetMenu(menu, language, maxDepth, viewFilter, traverseFilter));
        }

        /* Submenu */

        /// <summary>
        /// Gets a list of content page nodes (i.e. a tree) nested under the given content page.
        /// </summary>
        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static List<ContentPageNode> GetSubmenu(ContentPage parent, int maxDepth = -1, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                return TraverseContentPage(conn, parent.Language, parent.ID, 0, maxDepth, viewFilter, traverseFilter);
            }
        }

        /// <summary>
        /// Gets a IHierarchicalEnumerable tree of content page nodes nested under the given content page.
        /// </summary>
        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static ContentPageNodeView GetSubmenuView(ContentPage parent, int maxDepth = -1, Func<ContentPageNode, bool> viewFilter = null, Func<ContentPageNode, bool> traverseFilter = null)
        {
            return new ContentPageNodeView(GetSubmenu(parent, maxDepth, viewFilter, traverseFilter));
        }

        /* Path */

        /// <summary>
        /// Gets a list of Content Pages denoting the current content page's path.
        /// </summary>
        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static List<ContentPage> GetPath()
        {
            return GetPath(ContentPage.Current);
        }

        /// <summary>
        /// Gets a list of Content Pages denoting the given content page's path.
        /// </summary>
        [Obsolete("Please use the new Graph Navigation Framework.")]
        public static List<ContentPage> GetPath(ContentPage contentPage)
        {
            var path = new List<ContentPage>();
            BuildPathInternal(contentPage, path);
            return path;
        }

        /* Cache */

        public static void ClearCache()
        {
            lock (cacheKeys)
            {
                foreach (string cacheKey in cacheKeys)
                    HttpContext.Current.Cache.Remove(cacheKey);

                cacheKeys.Clear();
            }
        }
    }
}
