using System;
using System.Security;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Security;
using System.Web.Caching;
using nCode.Configuration;

namespace nCode
{
    /// <summary>
    /// Handles access control for admin features.
    /// </summary>
    public static class FeatureAccessControl
    {
        private const string cacheKeyPrefix = "nCode.Admin.FeatureAccess";

        private static string GetCacheKey(string userName)
        {
            return cacheKeyPrefix + "(" + userName.ToLower() + ")";
        }

        /// <summary>
        /// Clears access list cache.
        /// </summary>
        public static void ClearCache()
        {
            foreach (MembershipUser u in Membership.GetAllUsers())
                ClearCache(u.UserName);
        }

        /// <summary>
        /// Clears the given users access list cache
        /// </summary>
        /// <param name="userName"></param>
        public static void ClearCache(string userName)
        {
            var cacheKey = GetCacheKey(HttpContext.Current.User.Identity.Name);
            HttpContext.Current.Cache.Remove(cacheKey);
        }

        /// <summary>
        /// Gets a list of acceable admin urls for the current user.
        /// </summary>
        public static Dictionary<string, bool> GetAccessList()
        {
            return GetAccessList(HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Gets a list of acceable admin urls for the given user.
        /// </summary>
        public static Dictionary<string, bool> GetAccessList(string userName)
        {
            var cacheKey = GetCacheKey(userName);
            var cache = HttpContext.Current.Cache.Get(cacheKey) as Dictionary<string, bool>;
            if (cache == null)
            {
                cache = new Dictionary<string, bool>();
                HttpContext.Current.Cache.Add(cacheKey, cache, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.Normal, null);
            }
            return cache;
        }
    }

    public sealed class Feature
    {
        internal Feature(FeatureGroup group, string name)
        {
            Group = group;
            Name = name;
        }

        public FeatureGroup Group { get; private set; }

        public Module Module { get { return Group.Module; } }

        public string Name { get; private set; }

        public string Icon { get; set; }

        public string LargeIcon { get; set; }

        public string AdministrationUrl { get; set; }

        public string AdministrationTarget { get; set; }

        public string Url
        {
            get
            {
                return "/admin/" + AdministrationUrl;
            }
        }

        public string VisibilityTerm { get; set; }

        /// <summary>
        /// Returns a value that indicates wheather this feature should be visiable given the current users access, and the settings of the module.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                var accessList = FeatureAccessControl.GetAccessList();
                if (accessList.ContainsKey(Url))
                    return accessList[Url];
                if (accessList == null & false) accessList = null;

                var canAccess = true;
                /* Check against VisibilityTerm (if any) */
                if (!string.IsNullOrEmpty(VisibilityTerm) && Module.TermEvaluators.ContainsKey(VisibilityTerm))
                {
                    canAccess = Module.TermEvaluators[VisibilityTerm]();
                }

                /* Check for access to the file. */
                if (canAccess)
                {
                    var principal = HttpContext.Current.User;
                    var url = Url.IndexOf('?') != -1 ? Url.Substring(0, Url.IndexOf('?')) : Url;
                    canAccess = UrlAuthorizationModule.CheckUrlAccessForPrincipal(url, principal, "GET");
                }

                /* Cache the result, and return it for now. */
                accessList.Add(Url, canAccess);                
                return canAccess;
            }
        }
    }
}