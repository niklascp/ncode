using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using nCode.CMS;

namespace nCode.CMS.UI
{
    /// <summary>
    /// Represents a item int the ContentPageList control.
    /// </summary>
    public class ContentPageListItem : Control, IDataItemContainer
    {
        private ContentPage content;
        private int index;
        private int depth;
        private int displayIndex;

        public ContentPageListItem(ContentPage content, int index, int depth, int displayIndex)
        {
            this.content = content;
            this.index = index;
            this.depth = depth;
            this.displayIndex = displayIndex;
        }

        public object DataItem
        {
            get { return content; }
        }

        public int DataItemIndex
        {
            get { return index; }
        }

        public int DisplayIndex
        {
            get { return displayIndex; }
        }

        public int Depth
        {
            get { return depth; }
        }
    }
}
