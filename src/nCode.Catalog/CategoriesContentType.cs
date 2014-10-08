/* Obsolute Code - Disable Missing Comment Warnings. */
/* New Solutions should be build using the Navigation Framework. */
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCode.CMS.UI;
using System.Web.UI;
using nCode.CMS;
using System.Web.Compilation;
using nCode.Catalog.UI;
using System.Web.Routing;

namespace nCode.Catalog
{
    [Obsolete("Should be replaced by a PageType-system.")]
    public sealed class CategoriesContentType : nCode.CMS.ContentType
    {
        private static readonly Guid _id = new Guid("DEF0C473-9ECD-4E6E-94BE-2ADACA667DB8");
        private static readonly string name = "Category";

        private void AddChildren(ContentPageNode contentPage)
        {
            using (var catalogModel = new CatalogModel())
            {
                var categories = from c in catalogModel.Categories
                                 from l in c.Localizations.Where(x => x.Culture == contentPage.Language).DefaultIfEmpty()
                                 from g in c.Localizations.Where(x => x.Culture == null)
                                 where c.IsVisible
                                 orderby c.Index
                                 select new CategoryInfo(c.ID)
                                 {
                                     Title = (l ?? g).Title,
                                     CategoryNo = c.CategoryNo
                                 };

                foreach (var c in categories)
                {
                    ContentPageNode p = new ContentPageNode(c.ID);
                    p.ShowInMenu = true;
                    p.Title = c.Title;
                    p.Language = contentPage.Language;
                    p.LinkUrl = c.Url;
                    p.Permalink = c.Permalink;
                    p.IsEditable = false;
                    p.Parent = contentPage;
                }
            }
        }

        public override Guid ID
        {
            get { return _id; }
        }

        public override string Name
        {
            get { return name; }
        }

        public override string Title
        {
            get { return "Kategorier"; }
        }

        public override string Description
        {
            get { return "Lister automatisk katalog-modulets kategorier som undersider."; }
        }

        public override string Icon
        {
            get { return "~/admin/catalog/icons/categories.png"; }
        }

        public override ContentControl GetControl(Page page)
        {
            return null;
        }

        public override ContentEditControl GetEditControl(Page page)
        {
            return (ContentEditControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/ContentControls/Common/PrecedenceEdit.ascx", typeof(ContentEditControl));
        }

        public override void PreTraverseChildren(ContentPageNode contentPage)
        {
            if (contentPage.GetProperty<bool>("PrependPseudoChildren", false))
                AddChildren(contentPage);
        }

        public override void PostTraverseChildren(ContentPageNode contentPage)
        {
            if (!contentPage.GetProperty<bool>("PrependPseudoChildren", false))
                AddChildren(contentPage);
        }

        public override bool AllowChildren
        {
            get { return false; }
        }
    }
}
