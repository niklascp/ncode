﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lucene.Net.Store;
using LU = Lucene.Net.Util;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using nCode.UI;
using Lucene.Net.Search;
using System.Web;

namespace nCode.Search
{
    public class LuceneIndexUpdateContext : IIndexUpdateContext
    {
        LuceneSearchEngine searchEngine;
        private IndexWriter indexWriter;
        private IndexReader indexReader;
        private SearchSource searchSource;

        public LuceneIndexUpdateContext(LuceneSearchEngine engine, SearchSource source)
        {
            searchEngine = engine;
            var analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(LU.Version.LUCENE_30);
            indexWriter = new IndexWriter(engine.IndexDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
            indexReader = indexWriter.GetReader();
            searchSource = source;
        }

        /// <summary>
        /// Deletes all enties in this index.
        /// </summary>
        /// <param name="entryId"></param>
        public void FlushIndex()
        {
            var deleteQuery = new BooleanQuery();
            deleteQuery.Add(new TermQuery(new Term("source", searchSource.GetType().FullName)), Occur.MUST);
            indexWriter.DeleteDocuments(deleteQuery);
        }

        public void DeleteEntry(Guid entryId)
        {
            var deleteQuery = new BooleanQuery();
            deleteQuery.Add(new TermQuery(new Term("source", searchSource.GetType().FullName)), Occur.MUST);
            deleteQuery.Add(new TermQuery(new Term("id", entryId.ToString().ToLower())), Occur.MUST);
            indexWriter.DeleteDocuments(deleteQuery);
        }

        public void IndexEntry(SearchIndexEntry searchEntry)
        {
            /*
            var deleteQuery = new BooleanQuery();
            deleteQuery.Add(new TermQuery(new Term("source", searchSource.SourceGuid.ToString())), Occur.MUST);
            deleteQuery.Add(new TermQuery(new Term("id", searchEntry.Id.ToString())), Occur.MUST);
            //deleteQuery.Add(new TermQuery(new Term("culture", searchEntry.Culture)), Occur.MUST);
            indexWriter.DeleteDocuments(deleteQuery);
            */

            Document document = new Document();
            document.Add(new Field("source", searchSource.GetType().FullName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("id", searchEntry.Id.ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("url", searchEntry.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));

            if (searchEntry.Culture != null)
                document.Add(new Field("culture", searchEntry.Culture.ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            if (searchEntry.Title != null)
            {
                document.Add(new Field("title_analyzed", searchEntry.Title.ToLower(), Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("title_stored", searchEntry.Title, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            if (searchEntry.Description != null)
            {
                document.Add(new Field("description_analyzed", searchEntry.Description.ToLower(), Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("description_stored", searchEntry.Description, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            if (searchEntry.Content != null)
            {
                document.Add(new Field("content_analyzed", searchEntry.Content.ToLower(), Field.Store.NO, Field.Index.ANALYZED));
            }

            if (searchEntry.Keywords != null)
            {
                document.Add(new Field("keywords_analyzed", searchEntry.Keywords, Field.Store.NO, Field.Index.ANALYZED));
                document.Add(new Field("keywords_stored", searchEntry.Keywords, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            indexWriter.AddDocument(document);
        }

        public void CommitChanges()
        {
            try
            {
                indexWriter.Optimize();
            }
            catch (Exception ex)
            {
                Log.WriteEntry(EntryType.Error, "Search", "Optimize Index", exception: ex);
            }

            searchEngine.ReopenIndex();
        }

        public void Dispose()
        {
            try
            {
                indexWriter.Dispose();
            }
            catch (Exception ex)
            {
                Log.WriteEntry(EntryType.Error, "Search", "Close Index", exception: ex);
            }
        }
    }

}
