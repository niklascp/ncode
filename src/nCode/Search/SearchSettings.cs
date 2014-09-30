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
        static LuceneSearchSettings() {
            LuceneSearchSettings.LucenePath = HttpContext.Current.Server.MapPath(LuceneSearchSettings.LuceneVirtualPath);
            if (!Directory.Exists(LuceneSearchSettings.LucenePath))
            {
                Directory.CreateDirectory(LuceneSearchSettings.LucenePath);
            }
        }

        public static string LuceneVirtualPath = "~/Files/.SearchIndex";

        public static string LucenePath { get; internal set; }
    }
}
