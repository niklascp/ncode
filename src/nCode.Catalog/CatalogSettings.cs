using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog
{
    public static class CatalogSettings
    {
        private const string autoAssignItemNo = "nCode.Catalog.Settings.AutoAssignItemNo";

        /// <summary>
        /// Gets or sets a value indicating wheather Brands can be nested. Default value is false.
        /// </summary>
        public static bool AutoAssignItemNo
        {
            get { return Settings.GetProperty<bool>(autoAssignItemNo, false); }
            set { Settings.SetProperty<bool>(autoAssignItemNo, value); }
        }
    }
}
