using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.UI
{
    public class SearchResult : INavigationItem
    {
        public Guid ID { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public double Score { get; set; }

        public INavigationItem GetParent()
        {
            return null;
        }
    }
}
