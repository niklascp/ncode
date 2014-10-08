using nCode.CMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.CMS
{
    internal static class CmsPathMappingCache
    {
        private static object _lock = new object();
        private static Dictionary<string, Guid> map = null;
        private static Dictionary<Guid, string> reverseMap = null;

        public static IDictionary<string, Guid> PathMapping {
            get {
                if (map == null)
                    RefreshPathMapping();

                return map;
            }
        }

        public static IDictionary<Guid, string> ReverseMapping
        {
            get
            {
                if (reverseMap == null)
                    RefreshPathMapping();

                return reverseMap;
            }
        }

        public static void RefreshPathMapping()
        {
            lock (_lock)
            {
                try
                {
                    using (var db = new CmsDbContext())
                    {
                        /* Allocate new space to maps. */
                        var m = new Dictionary<string, Guid>();
                        var rm = new Dictionary<Guid, string>();

                        var paths = db.ContentPages
                            .Where(x =>
                                (x.ParentID != null && x.Culture != null) &&
                                (x.ValidFrom == null || x.ValidFrom <= DateTime.Today) &&
                                (x.ValidTo == null || DateTime.Today < x.ValidTo))
                            .Select(x => new
                            {
                                x.ID,
                                x.Culture,
                                x.StaticPath
                            }).ToList();

                        var defaultCulture = Settings.SupportedCultureNames.FirstOrDefault();

                        foreach (var p in paths)
                        {
                            var path = (p.Culture.Equals(defaultCulture) ? string.Empty : p.Culture.ToLower() + "/") + p.StaticPath;
                            var invariantPath = path.ToLowerInvariant();

                            if (!string.IsNullOrEmpty(path) && !m.ContainsKey(invariantPath))
                            {
                                m.Add(invariantPath, p.ID);
                                rm.Add(p.ID, path);
                            }
                        }

                        Log.Info("Refreshed CMS Path Mapping Cache. Total Items: " + m.Count);

                        map = m;
                        reverseMap = rm;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Log.Error("Refreshed CMS Path Mapping Cache failed.", ex);
                }
            }
        }
    }
}
