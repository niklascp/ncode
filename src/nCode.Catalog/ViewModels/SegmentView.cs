using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nCode.Metadata;
using nCode.UI;

namespace nCode.Catalog.ViewModels
{
    public class SegmentView : IItemListContextView, ISeoData
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
    }
}
