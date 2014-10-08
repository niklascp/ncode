using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCode.CMS.UI;
using System.Web.UI;
using nCode.CMS;
using System.Web.Compilation;

namespace nCode.Catalog
{
    [Obsolete("Should be replaced by a PageType-system.")]
    public sealed class BrandsContentType : nCode.CMS.ContentType
    {
        private static readonly Guid id = new Guid("27C6439A-999C-4919-ABD9-2A58D34F0C89");
        private static readonly string name = "Brand";

        private void AddChildren(ContentPageNode contentPage)
        {
            using (var catalogModel = new CatalogModel())
            {
                var brands = from b in catalogModel.Brands
                             from l in b.Localizations.Where(x => x.Culture == contentPage.Language).DefaultIfEmpty()
                             from g in b.Localizations.Where(x => x.Culture == null)
                             orderby b.Index
                             select new
                             {
                                 b.ID,
                                 b.Name
                             };

                foreach (var s in brands)
                {
                    ContentPageNode p = new ContentPageNode(s.ID);
                    p.ShowInMenu = true;
                    p.Title = s.Name;
                    p.Language = contentPage.Language;
                    p.LinkUrl = "/" + p.Language.ToLower() + "/catalog/Item-List?Brand=" + s.ID;
                    p.Permalink = "/" + p.Language.ToLower() + "/catalog/Item-List?Brand=" + s.ID;
                    p.IsEditable = false;
                    p.Parent = contentPage;
                }
            }
        }

        public override Guid ID
        {
            get { return id; }
        }

        public override string Name
        {
            get { return name; }
        }

        public override string Title
        {
            get { return "Mærker"; }
        }

        public override string Description
        {
            get { return "Lister automatisk katalog-modulets mærker som undersider."; }
        }

        public override string Icon
        {
            get { return "~/admin/catalog/icons/brands.png"; }
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
