using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    [Serializable]
    public class TreeItem
    {
        public string Path;
        public string Name;
        public string ContextMenu;
        public string Icon;
        public bool HasChildren;

        [Obsolete("Use HasChildren")]
        public bool HasChilds
        {
            get { return HasChildren; }
            set { HasChildren = value; }
        }
    }
}
