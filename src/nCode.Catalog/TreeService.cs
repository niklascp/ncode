using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using nCode.Catalog;
using nCode.UI;

namespace nCode.Catalog
{
    /// <summary>
    /// Summary description for TreeService
    /// </summary>
    [ScriptService]
    public class TreeService : System.Web.Services.WebService
    {
        private static List<TreeItem> GetCategoriesInternal(Guid? parentId, bool withItems)
        {
            using (var catalogModel = new CatalogModel())
            {
                var treeItems = (from c in catalogModel.Categories
                                 from g in c.Localizations.Where(x => x.Culture == null)
                                 where parentId != null ? c.ParentID == parentId : c.ParentID == null
                                 orderby c.Index
                                 select new TreeItem()
                                 {
                                     Path = "C:" + c.ID.ToString().ToLower(),
                                     Name = g.Title,
                                     HasChildren = catalogModel.Categories.Any(x => x.ParentID == c.ID) || (withItems && catalogModel.Items.Any(x => x.CategoryID == c.ID)),
                                     ContextMenu = "categoryContextMenu"
                                 }).ToList();

                /* Request for root childs */
                if (parentId == null)
                {
                    treeItems.Add(new TreeItem()
                    {
                        Path = "C:" + Guid.Empty.ToString(),
                        Name = "Ikke kategoriseret",
                        Icon = "/admin/catalog/icons/uncategorized.png",
                        HasChildren = withItems && catalogModel.Items.Any(x => x.CategoryID == null)
                    });
                }

                if (withItems)
                {
                    treeItems.AddRange((from i in catalogModel.Items
                                        from g in i.Localizations.Where(x => x.Culture == null)
                                        where parentId != Guid.Empty ? i.CategoryID == parentId : i.CategoryID == null
                                        orderby i.Index
                                        select new TreeItem()
                                        {
                                            Path = "I:" + i.ID.ToString().ToLower(),
                                            Name = g.Title,
                                            ContextMenu = "itemContextMenu"
                                        }).ToList());
                }

                return treeItems;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<TreeItem> GetTreeWithItems(string path)
        {
            using (var catalogModel = new CatalogModel())
            {
                /* Add Root Element */
                if (string.IsNullOrEmpty(path))
                {
                    /* TODO: When uncategorized/non-branded items is shown in root, replace HasChildren with catalogModel.Categories.Any() and catalogModel.Brands.Any() */
                    var rootItems = new List<TreeItem>();
                    rootItems.Add(new TreeItem() { Name = "Kategorier", Icon = "/admin/catalog/icons/categories.png", Path = "C:", HasChildren = true });
                    //rootItems.Add(new TreeItem() { Name = "Mærker", Icon = "/admin/catalog/icons/brands.png", Path = "B:", HasChilds = true });

                    return rootItems;
                }
                else
                {
                    var parts = path.Split(new char[] { ':' }, 2);

                    if (parts.Length < 2)
                        throw new ArgumentException("Invalid path.");

                    if (string.Equals(parts[0], "C"))
                    {
                        var parentId = (!string.IsNullOrEmpty(parts[1]) ? new Guid(parts[1]) : (Guid?)null);
                        return GetCategoriesInternal(parentId, true);
                    }
                    //else if (string.Equals(parts[0], "B"))
                    //{
                    //    return GetBrandsInternal("B:");
                    //}
                    else
                    {
                        throw new ArgumentException("Unknown prefix '" + parts[0] + "'");
                    }
                }
            }
        }
    }
}
