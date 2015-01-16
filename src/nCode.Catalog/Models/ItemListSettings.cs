using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.Models
{
    public class ItemListSettings
    {
        public ItemListSettings()
        {
            ShowTitle = true;
            ShowItemNo = false;
            ShowBrandName = false;
            ShowPrice = true;
            ShowDefaultPrice = false;
        }

        public bool ShowTitle { get; set; }

        public bool ShowItemNo { get; set; }

        public bool ShowBrandName { get; set; }

        public bool ShowPrice { get; set; }

        public bool ShowDefaultPrice { get; set; }
    }
}
