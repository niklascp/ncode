using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.Search
{
    public static class SearchHandler
    {
        private static object syncRoot;
        private static bool isInitialized;
        private static List<SearchSource> sources;
        private static SearchEngine engine;

        static SearchHandler()
        {
            syncRoot = new object();
            isInitialized = false;
        }

        public static bool IsInitialized { get { return isInitialized; } }

        public static void Initialize(SearchEngine searchEngine)
        {
            if (searchEngine == null)
                throw new ArgumentNullException("searchEngine");

            Log.Info("Initializing Search Engine: " + searchEngine.GetType().FullName + ".");

            lock (syncRoot)
            {
                if (isInitialized)
                    throw new ApplicationException("The Search Engine has already been initialized.");

                sources = new List<SearchSource>();
                engine = searchEngine;

                isInitialized = true;

                Log.Info("Search Engine Initialized.");
            }
        }

        public static SearchEngine Engine
        {
            get { return engine; }
        }

        public static IList<SearchSource> Sources
        {
            get { return sources.AsReadOnly(); }
        }

        public static void AddSource(SearchSource source)
        {
            sources.Add(source);
        }
    }
}
