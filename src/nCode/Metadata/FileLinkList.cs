using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    [EditControl("~/Admin/System/MetadataEditControls/FileLinkList.ascx")]
    public class FileLinkList : List<FileLinkListItem>
    {

    }

    public class FileLinkListItem : FileListItem
    {
        public string Link { get; set; }
    }
}
