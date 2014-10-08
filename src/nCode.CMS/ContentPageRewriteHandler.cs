using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace nCode.CMS
{
    /// <summary>
    /// Rewites links in content to SEO-friendly style.
    /// </summary>
    public class ContentPageRewriteHandler : ContentRewriteHandler
    {
        private readonly Regex urlRegex;
        private readonly Regex regex;

        /// <summary>
        /// Creates a new ContentPageRewriteHandler.
        /// </summary>
        public ContentPageRewriteHandler()
        {
            urlRegex = new Regex(@"^/cms/Content-View(?:\.aspx)?\?ID=([^&]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
   
            regex = new Regex(@"(?<=<A.+HREF\s*=\s*[""'])/cms/Content-View(?:\.aspx)?\?ID=([^'"" >]*)(?=[ '""][^>]*>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private string GetSeoFrindlyUrl(Guid contentPageId)
        {
            if (CmsPathMappingCache.ReverseMapping.ContainsKey(contentPageId))
                return "/" + CmsPathMappingCache.ReverseMapping[contentPageId];

            return null;
        }

        /// <summary>
        /// Gets the Regex used to match this handler against content.
        /// </summary>
        public override Regex Regex
        {
            get { return regex; }
        }

        /// <summary>
        /// Rewrites match in content before rendering.
        /// </summary>
        public override string RewritePreRenderMatch(Match match)
        {
            Guid contentPageId;
            if (Guid.TryParse(match.Groups[1].Value, out contentPageId))
                return GetSeoFrindlyUrl(contentPageId) ?? match.Value;
            else
                return match.Value;
        }

        public override string RewriteUrl(string url)
        {
            var match = urlRegex.Match(url);

            Guid contentPageId;

            if (match.Success && Guid.TryParse(match.Groups[1].Value, out contentPageId))
                return GetSeoFrindlyUrl(contentPageId) ?? url;

            return base.RewriteUrl(url);
        }
    }
}
