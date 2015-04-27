using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    /// <summary>
    /// Represents a interface for providing data for Search Engine Optimizations.
    /// </summary>
    public interface ISeoData
    {
        string Title { get; set; }

        string SeoKeywords { get; set; }

        string SeoDescription { get; set; }
    }
}
