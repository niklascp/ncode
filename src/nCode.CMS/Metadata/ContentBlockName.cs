using nCode.CMS.UI;
using nCode.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.CMS.Metadata
{
    [EditControl("~/Admin/CMS/MetadataEditControls/ContentBlock.ascx")]
    public class ContentBlockReference
    {
        /// <summary>
        /// Backward compability - initially the ContentBlock edit control saved the name of the selected ContentBlock simply as a string.
        /// </summary>
        public static implicit operator ContentBlockReference(string s)
        {
            return new ContentBlockReference() { ContentBlockCode = s };
        }

        /// <summary>
        /// Backward compability - initially the ContentBlock edit control saved the name of the selected ContentBlock simply as a string.
        /// </summary>
        public static implicit operator string(ContentBlockReference s)
        {
            return s.ContentBlockCode;
        }

        /// <summary>
        /// Gets or sets the name of the Referenced contentblock.
        /// </summary>
        public string ContentBlockCode { get; set; }

        public string GetContent(string culture = null, bool stripTrailingSpace = false)
        {
            return ContentBlockUtilities.GetContentBlockContent(ContentBlockCode, culture, stripTrailingSpace);
        }
    }
}
