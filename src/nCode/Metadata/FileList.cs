using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    [EditControl("~/Admin/System/MetadataEditControls/FileList.ascx")]
    public class FileList : List<FileListItem>
    {

    }

    public class FileListItem
    {
        public string File { get; set; }
    }
}
