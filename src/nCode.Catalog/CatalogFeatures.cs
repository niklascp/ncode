using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog
{
    public static class CatalogFeatures
    {
        private const string brandHierarchyKey = "nCode.Catalog.Features.BrandHierarchy";

        /// <summary>
        /// Gets or sets a value indicating wheather Brands can be nested. Default value is false.
        /// </summary>
        public static bool BrandHierarchy
        {
            get { return Settings.GetProperty<bool>(brandHierarchyKey, false); }
            set { Settings.SetProperty<bool>(brandHierarchyKey, value); }
        }
    }

}
