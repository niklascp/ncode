using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    [EditControl("~/Admin/System/MetadataEditControls/FileTextList.ascx")]
    public class FileTextList : List<FileTextListItem>
    {

    }

    public class FileTextListItem
    {
        public string File { get; set; }
        public string Text { get; set; }
    }
}
