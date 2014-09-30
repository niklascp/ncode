using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    public interface INavigationGraph
    {
        IEnumerable<INavigationItem> Roots { get; }

        IEnumerable<INavigationItem> Expand(INavigationItem item);
    }
}
