using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using nCode.CMS;
using nCode.CMS.Models;
using nCode.Metadata;
using nCode.UI;

using Dapper;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Represents a the Content Page Tree in the Navigation Framework.
    /// </summary>
    public class ContentPageNavigationTree : NavigationTree<ContentPageEntity, ContentPageNavigationItem>
    {
        #region SQL Queries
        const string contentPageQuery = @"
with cte as
(
	select			
		[RootID] = cp.[ID],
        [ID] = cp.[ID]
	from
		[CMS_ContentPage] cp
	where
		[Language] is null and [Title] = @menu
	union all
	select
        cte.[RootID],
		cp.[ID]
	from
		[CMS_ContentPage] cp
		JOIN cte ON cte.[ID] = cp.[ParentID]
)
select
	[ID] = cp.[ID],
    [ParentID] = case when cp.[ParentID] <> r.[RootID] then cp.[ParentID] end,
    [Title] = cp.[Title],
    [ShowInMenu] = cp.[ShowInMenu],
    [ValidFrom] = cp.[ValidFrom],
    [ValidTo] = cp.[ValidTo],
    [LinkUrl] = cp.[LinkUrl],
    [LinkMode] = cp.[LinkMode]
from
	[CMS_ContentPage] cp
	join [cte] r on r.[ID] = cp.[ID]
where
	cp.[Language] = @culture and
    cp.[ShowInMenu] = 1 and
    (cp.[ValidFrom] is null or cp.[ValidFrom] <= @today) and
    (cp.[ValidTo] is null or @today <= cp.[ValidTo])
order by
    cp.[DisplayIndex];
";
        #endregion

        public string Menu { get; private set; }

        public ContentPageNavigationTree(string menu, Func<ContentPageNavigationItem, bool> viewFilter = null, Func<ContentPageNavigationItem, bool> traverseFilter = null)
        {
            Menu = menu;
            ViewFilter = viewFilter;
            TraverseFilter = traverseFilter;
        }

        protected override IQueryable<ContentPageNavigationItem> InitializeSource()
        {
            using (var db = new CmsDbContext())
            {
                return db.Database.Connection.Query<ContentPageNavigationItem>(contentPageQuery, new { 
                        menu = Menu,
                        culture = CultureInfo.CurrentUICulture.Name,
                        today = DateTime.Today
                    }).AsQueryable();                
            }
        }
    }
}