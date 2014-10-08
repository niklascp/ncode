using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using nCode.Metadata;
using nCode.UI;

using nCode.CMS;
using nCode.CMS.Model;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Represents a the Content Page Tree in the Navigation Framework.
    /// </summary>
    public class ContentPageNavigationTree : NavigationTree<ContentPageEntity, ContentPageNavigationItem>
    {
        public string Menu { get; private set; }

        public ContentPageNavigationTree(string menu, Expression<Func<ContentPageEntity, bool>> sourceFilter = null, Func<ContentPageNavigationItem, bool> viewFilter = null, Func<ContentPageNavigationItem, bool> traverseFilter = null)
            : base(
                sourceFilter ?? (x => x.ShowInMenu && (x.ValidFrom == null || x.ValidFrom <= DateTime.Today) && (x.ValidTo == null || DateTime.Today <= x.ValidTo)),
                viewFilter,
                traverseFilter)
        {
            Menu = menu;
        }

        protected override IQueryable<ContentPageNavigationItem> InitializeSource()
        {
            using (var db = new CmsDbContext())
            {
                return (from p in db.ContentPages.Where(SourceFilter)
                        orderby p.DisplayIndex
                        select new ContentPageNavigationItem()
                        {
                            ID = p.ID,
                            ParentID = p.ParentID,
                            Title = p.Title,
                            ShowInMenu = p.ShowInMenu,
                            ValidFrom = p.ValidFrom,
                            ValidTo = p.ValidTo,
                            LinkUrl = p.LinkUrl,
                            LinkMode = p.LinkMode
                        }).ToList().AsQueryable();
            }
        }
    }
}