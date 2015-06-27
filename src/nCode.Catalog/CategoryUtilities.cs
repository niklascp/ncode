/* Obsolete Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

using System.Web;
using System.Web.UI;
using System.Globalization;
using nCode.Catalog.UI;
using nCode.UI;

namespace nCode.Catalog
{
    public delegate bool CategoryInfoFilter(CategoryInfo category);

    public delegate bool CategoryInfoNodeFilter(CategoryInfoNode category);
    
    public static class CategoryUtilities
    {
        const string currentCategoryPathKey = "nCode.Catalog.CurrentCategoryPath";

        private static IList<CategoryInfoNode> GetCategoryTreeInternal(CatalogModel model, string language, CategoryInfoNodeFilter viewFilter, CategoryInfoNodeFilter traverseFilter, CategoryInfoNode parentNode, int depth) 
        {
            var parentId = parentNode != null ? parentNode.ID : (Guid?)null;
            List<CategoryInfoNode> childs = new List<CategoryInfoNode>();

            var categories = (from c in model.Categories
                              from l in c.Localizations.Where(x => x.Culture == language).DefaultIfEmpty()
                              from g in c.Localizations.Where(x => x.Culture == null)
                              where parentId != null ? c.ParentID == parentId : c.ParentID == null
                              orderby c.Index
                              select new
                              {
                                  c.ID,
                                  c.CategoryNo,
                                  (l ?? g).Title,
                                  c.IsVisible,
                                  c.Image1,
                                  c.Image2,
                                  c.Image3
                              }).ToList();

            foreach (var c in categories)
            {
                CategoryInfoNode node = new CategoryInfoNode(c.ID);
                node.Depth = depth;
                node.CategoryNo = c.CategoryNo;
                node.Title = c.Title;
                node.IsVisible = c.IsVisible;
                node.Image1 = c.Image1;
                node.Image2 = c.Image2;
                node.Image3 = c.Image3;

                if (viewFilter == null || viewFilter(node))
                {
                    childs.Add(node);

                    if (parentNode != null)
                        parentNode.AddChild(node);
                }
            }

            foreach (CategoryInfoNode node in childs)
            {
                if (traverseFilter == null || traverseFilter(node))
                    GetCategoryTreeInternal(model, language, viewFilter, traverseFilter, node, depth + 1);
            }

            return childs.AsReadOnly();
        }

        [Obsolete("Please use the new Navigation Framework.")]
        public static IList<CategoryInfoNode> GetCategoryTree(string language = null, CategoryInfoNodeFilter viewFilter = null, CategoryInfoNodeFilter traverseFilter = null, CategoryInfoNode parentNode = null)
        {
            using (var model = new CatalogModel())
            {
                return GetCategoryTreeInternal(model, language, viewFilter, traverseFilter, parentNode, 0);
            }
        }

        [Obsolete("Please use the new Navigation Framework.")]
        public static CategoryInfo GetCategoryInfo(Guid id)
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                var category = (from c in model.Categories
                                join cg in model.CategoryLocalizations.Where(x => x.Culture == null) on c.ID equals cg.CategoryID
                                join cl in model.CategoryLocalizations.Where(x => x.Culture == CultureInfo.CurrentUICulture.Name) on c.ID equals cl.CategoryID into cls
                                from cl in cls.DefaultIfEmpty()
                                where c.ID == id
                                select new
                                {
                                    c.ID,
                                    c.CategoryNo,
                                    c.ParentID,
                                    (cl ?? cg).Title,
                                    c.Image1,
                                    c.Image2,
                                    c.Image3
                                }).Single();

                return new CategoryInfo(category.ID) {
                    CategoryNo = category.CategoryNo,
                    Title = category.Title,
                    Image1 = category.Image1,
                    Image2 = category.Image2,
                    Image3 = category.Image3
                };
            }
        }

        [Obsolete("Please use the new Navigation Framework.")]
        public static IList<CategoryInfo> CurrentCategoryPath
        {
            get
            {
                var currentPath = HttpContext.Current.Items[currentCategoryPathKey] as List<CategoryInfo>;
                if (currentPath == null)
                {
                    currentPath = new List<CategoryInfo>();
                }
                return currentPath.AsReadOnly();
            }
            private set
            {
                HttpContext.Current.Items[currentCategoryPathKey] = value;
            }
        }

        [Obsolete("Please use the new Navigation Framework.")]
        public static void SetCurrentCategory(Guid? ID)
        {
            if (ID != null)
                Navigation.Current = CategoryNavigationItem.GetFromID(ID.Value);
            else
                Navigation.Current = null;
        }
    }
}
