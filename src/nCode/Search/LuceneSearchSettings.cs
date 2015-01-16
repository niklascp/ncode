using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.Search
{
    public static class LuceneSearchSettings
    {
        private const string useFuzzySearchKey = "nCode.Search.Lucene.UseFuzzySearch";

        static LuceneSearchSettings() {
            LuceneSearchSettings.LucenePath = HttpContext.Current.Server.MapPath(LuceneSearchSettings.LuceneVirtualPath);
            if (!Directory.Exists(LuceneSearchSettings.LucenePath))
            {
                Directory.CreateDirectory(LuceneSearchSettings.LucenePath);
            }
        }

        public static string LuceneVirtualPath = "~/Files/.SearchIndex";

        public static string LucenePath { get; internal set; }

        public static bool UseFuzzySearch
        {
            get { return Settings.GetProperty(useFuzzySearchKey, true); }
            set { Settings.SetProperty(useFuzzySearchKey, value); }
        }
    }
}
