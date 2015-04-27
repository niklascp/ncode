using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.UI
{
    public class ItemViewRequest
    {
        public string ItemNo { get; set; }
        public string OriginalPath { get; set; }
        public string CanonicalPath { get; set; }
    }
}
