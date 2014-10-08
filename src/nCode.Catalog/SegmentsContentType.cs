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
    public sealed class SegmentsContentType : nCode.CMS.ContentType
    {        
        private static readonly Guid _id = new Guid("63F8F6BE-1EF9-4CDB-8762-0044C117B19E");
        private static readonly string name = "Segment";

        private void AddChildren(ContentPageNode contentPage)
        {
            using (var catalogModel = new CatalogModel())
            {
                var segments = from s in catalogModel.Segments
                               from l in s.Localizations.Where(x => x.Culture == contentPage.Language).DefaultIfEmpty()
                               from g in s.Localizations.Where(x => x.Culture == null)
                               where s.IsActive
                               orderby s.DisplayIndex
                               select new
                               {
                                   s.ID,
                                   (l ?? g).Title
                               };

                foreach (var s in segments)
                {
                    ContentPageNode p = new ContentPageNode(s.ID);
                    p.ShowInMenu = true;
                    p.Title = s.Title;
                    p.Language = contentPage.Language;
                    p.LinkUrl = "/" + p.Language.ToLower() + "/catalog/Item-List?Segment=" + s.ID;
                    p.Permalink = "/" + p.Language.ToLower() + "/catalog/Item-List?Segment=" + s.ID;
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
            get { return "Målgrupper"; }
        }

        public override string Description
        {
            get { return "Lister automatisk katalog-modulets målgrupper som undersider."; }
        }

        public override string Icon
        {
            get { return "~/admin/catalog/icons/segments.png"; }
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
