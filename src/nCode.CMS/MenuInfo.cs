using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Caching;

namespace nCode.CMS
{
    /*
    public class MenuInfo : IPageContainer
    {
        private Guid id;
        private string title;

        internal MenuInfo(Guid id, string title)
        {
            this.id = id;
            this.title = title;
        }

        private string GetCacheKey(string language, bool onlyVisible)
        {
            return Utilities.CacheKeyPrefix + "_" + ID.ToString() + "_" + language.ToLower() + (onlyVisible ? "_visible" : "_all");
        }

        /// <summary>
        /// Gets the unique ID of this Menu.
        /// </summary>
        public Guid ID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the title of this Menu.
        /// </summary>
        public string Title
        {
            get { return title; }
        }

        /// <summary>
        /// Gets this Containers parent object. This value is always null.
        /// </summary>
        public IPageContainer Parent
        {
            get { return null; }
        }

        /// <summary>
        /// Gets this Menu's visible child pages in the language specified by CultureInfo.CurrentUICulture.
        /// </summary>
        public IList<PageInfo> Childs
        {
            get
            {
                return GetPages(CultureInfo.CurrentUICulture.Name, true);
            }
        }

        /// <summary>
        /// Gets the cached list of pages associated with this menu. If the cache does
        /// not exist it is automatic rebuild.
        /// </summary>
        /// <param name="language">The Language the pages in.</param>
        /// <param name="onlyVisible">Indicates whether the returned collection should only include visible pages.</param>
        public IList<PageInfo> GetPages(string language, bool onlyVisible)
        {
            // Check the cache
            string cacheKey = GetCacheKey(language, onlyVisible);
            IList<PageInfo> pages = HttpRuntime.Cache[cacheKey] as IList<PageInfo>;

            // No Cache - Build Pages and add to Cache
            if (pages == null)
            {
                pages = PageInfo.GetPages(this, language, onlyVisible);

                // Cache must expire at midnight, as pages could become
                // invalid or valid over midnight due to ValidFrom / ValidTo property.
                HttpRuntime.Cache.Add(cacheKey, pages, null, DateTime.Today.AddDays(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }

            return pages;
        }

        /// <summary>
        /// Clears the cache for the given language.
        /// </summary>
        public void ClearCache(string language) {
            HttpRuntime.Cache.Remove(GetCacheKey(language, true));
            HttpRuntime.Cache.Remove(GetCacheKey(language, false));
        }
    }

    public interface IPageContainer
    {
        Guid ID { get; }
        IPageContainer Parent { get; }
        IList<PageInfo> Childs { get; }
    }
    */
}