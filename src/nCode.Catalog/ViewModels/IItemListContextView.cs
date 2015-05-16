using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nCode.Metadata;
using nCode.UI;

namespace nCode.Catalog.ViewModels
{
    public interface IItemListContextView : ISeoData
    {
        string Title { get; set; }
        string Description { get; set; }
        string SeoDescription { get; set; }
        string SeoKeywords { get; set; }        
    }
}
