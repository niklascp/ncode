using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    public interface ISearchProvider
    {
        IEnumerable<SearchResult> Search(string searchQuery);
    }
}
