using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using nCode.UI;

using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using LS = Lucene.Net.Store;
using LU = Lucene.Net.Util;

namespace nCode.Search
{
    public class LuceneSearchEngine : SearchEngine
    {
        /* Lucene Pointers */
        public LS.Directory IndexDirectory { get; private set; }
        private IndexSearcher indexSearcher;

        public LuceneSearchEngine(HttpApplication app)
        {
            IndexDirectory = LS.FSDirectory.Open(LuceneSearchSettings.LucenePath);

            try
            {
                indexSearcher = new IndexSearcher(IndexDirectory);
            }
            catch (FileNotFoundException)
            {

            }
        }

        public override int GetEntryCount(SearchSource searchSource)
        {
            if (indexSearcher == null)
                return 0;

            var reader = indexSearcher.IndexReader;

            return reader.DocFreq(new Term("source", searchSource.GetType().FullName));
        }

        public override IEnumerable<SearchResult> Search(string queryString)
        {
            var results = new List<SearchResult>(100);

            if (queryString == null || indexSearcher == null)
                return results;

            var query = new BooleanQuery();

            var words = queryString.ToLower().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var w in words) {

                var subquery = new BooleanQuery();

                var titlePrefixQuery = new Lucene.Net.Search.PrefixQuery(new Term("title_analyzed", w));
                var titleFuzzyQuery = new Lucene.Net.Search.FuzzyQuery(new Term("title_analyzed", w), 0.5f, 3);

                var descriptionQuery = new Lucene.Net.Search.FuzzyQuery(new Term("description_analyzed", w), 0.7f, 3);
                var keywordQuery = new Lucene.Net.Search.FuzzyQuery(new Term("keywords_analyzed", w), 0.7f, 3);

                var contentQuery = new Lucene.Net.Search.TermQuery(new Term("content_analyzed", w));

                subquery.Add(titlePrefixQuery, Occur.SHOULD);
                subquery.Add(titleFuzzyQuery, Occur.SHOULD);
                subquery.Add(descriptionQuery, Occur.SHOULD);
                subquery.Add(keywordQuery, Occur.SHOULD);
                subquery.Add(contentQuery, Occur.SHOULD);

                query.Add(subquery, Occur.MUST);
            }

            var hits = indexSearcher.Search(query, 100);            

            //iterate over the results.
            for (int i = 0; i < hits.ScoreDocs.Length; i++)
            {
                Document doc = indexSearcher.Doc(hits.ScoreDocs[i].Doc);
                
                var result = new SearchResult()
                {
                    ID = new Guid(doc.Get("id")),
                    Title = doc.Get("title_stored"),
                    Description = doc.Get("description_stored"),
                    Url = doc.Get("url"),
                    Score = hits.ScoreDocs[i].Score
                };

                results.Add(result);
            }

            return results;
        }

        public override IIndexUpdateContext GetUpdateContext(SearchSource searchSource)
        {
            return new LuceneIndexUpdateContext(this, searchSource);
        }

        public void ReopenIndex() {
            if (indexSearcher != null)
                indexSearcher.Dispose();
            indexSearcher = new IndexSearcher(IndexDirectory);
        }
    }
}
