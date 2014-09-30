using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nCode
{
    /// <summary>
    /// Abstraction for Content Rewrite Handlers
    /// </summary>
    public abstract class ContentRewriteHandler
    {
        /// <summary>
        /// Gets the Regex used to match this handler against content.
        /// </summary>
        public abstract Regex Regex { get; }

        /// <summary>
        /// Rewrites match in content before rendering.
        /// </summary>
        public abstract string RewritePreRenderMatch(Match match);

        /// <summary>
        /// Rewrites match in content before edit.
        /// </summary>
        public virtual string RewritePreEditMatch(Match match) { return match.ToString(); }

        /// <summary>
        /// Rewrites the url.
        /// </summary>
        public virtual string RewriteUrl(string url) { return url; }
    }

    /// <summary>
    /// Handles Content Rewrite
    /// </summary>
    public static class ContentRewriteControl
    {
        private static List<ContentRewriteHandler> handlers;

        static ContentRewriteControl()
        {
            handlers = new List<ContentRewriteHandler>();
        }

        /// <summary>
        /// Adds a new handler.
        /// </summary>
        public static void AddHandler(ContentRewriteHandler handler) 
        {
            handlers.Add(handler);
        }

        /// <summary>
        /// Rewrites the given url using the loaded Rewrite Hadnlers.
        /// </summary>
        public static string RewriteUrl(string url)
        {
            foreach (var handler in handlers)
            {
                url = handler.RewriteUrl(url);
            }

            return url;
        }

        /// <summary>
        /// Rewrites the content before rendering.
        /// </summary>
        public static string RewritePreRenderContent(string content)
        {
            if (content == null)
                return null;

            foreach (var handler in handlers)
            {
                if (handler.Regex == null)
                    continue;
                content = handler.Regex.Replace(content, new MatchEvaluator(handler.RewritePreRenderMatch));
            }

            return content;
        }
    }
}
