using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Web.Caching;
using System.Web;

using nCode.Configuration;
using nCode.UI;
using System.Text.RegularExpressions;

namespace nCode.CMS.UI
{
    public static class ContentBlockUtilities
    {
        private const string cachePrefix = "nCode.CMS.ContentBlock.";
        private const string trailingSpacePattern = @"(<p>(&nbsp;|\s)*</p>(&nbsp;|\s)*)+$";

        private static string GetCacheKey(string code, string culture)
        {   
            return cachePrefix + CultureInfo.InvariantCulture.TextInfo.ToTitleCase(code) + "(" + culture + ")";
        }

        public static void ClearContentBlockCache(string code)
        {
            foreach (SupportedCulture culture in GlobalizationSection.GetSection().SupportedCultures)
                ClearContentBlockCache(code, culture.Name);
        }
        
        public static void ClearContentBlockCache(string code, string culture)
        {
            string cacheKey = GetCacheKey(code, culture);
            HttpContext.Current.Cache.Remove(cacheKey);
        }

        public static string GetContentBlockContent(string code, string culture = null, bool stripTrailingSpace = false)
        {

            if (culture == null)
                culture = CultureInfo.CurrentUICulture.Name;

            string cacheKey = GetCacheKey(code, culture);

            string cache = HttpContext.Current.Cache.Get(cacheKey) as string;

            if (cache != null)
                return cache;

            using (CmsModel model = new CmsModel(SqlUtilities.ConnectionString))
            {
                cache = (from cb in model.ContentBlocks
                         from l in cb.Localizations.Where(x => x.Culture == culture).DefaultIfEmpty()
                         from g in cb.Localizations.Where(x => x.Culture == null)
                         where cb.Code == code
                         select (l ?? g).Content).SingleOrDefault();

                cache = ContentRewriteControl.RewritePreRenderContent(cache);

                if (cache != null)
                {
                    if (stripTrailingSpace)
                        cache = Regex.Replace(cache, trailingSpacePattern, string.Empty);
                    
                    HttpContext.Current.Cache.Add(cacheKey, cache, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.Default, null);
                }
            }

            return cache;
        }
    }
}
