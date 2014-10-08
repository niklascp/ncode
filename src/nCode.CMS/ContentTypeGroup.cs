using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace nCode.CMS
{
    /// <summary>
    /// Represents a Group of Content Types (e.g. Content Types for Intranet)
    /// </summary>
    public class ContentTypeGroup : List<ContentType>
    {
        private ContentTypeCollection contentTypes;
        private string name;

        public ContentTypeGroup(ContentTypeCollection contentTypes)
        {
            this.contentTypes = contentTypes;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public new void Add(ContentType contentType)
        {
            if (contentTypes.contentTypesByID.ContainsKey(contentType.ID))
            {
                contentTypes.Replace(contentType.ID, contentType);
            }
            else
            {
                contentTypes.contentTypesByID.Add(contentType.ID, contentType);
                base.Add(contentType);
            }               
        }
    }
}
