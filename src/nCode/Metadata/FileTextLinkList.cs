using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    [EditControl("~/Admin/System/MetadataEditControls/FileTextLinkList.ascx")]
    public class FileTextLinkList : List<FileTextLinkListItem>
    {

    }

    public class FileTextLinkListItem : FileLinkListItem
    {
        public string Link { get; set; }
    }
}
