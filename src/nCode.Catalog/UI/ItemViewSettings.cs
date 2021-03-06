﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.UI
{
    /// <summary>
    /// Represents settings for an item detail view.
    /// </summary>
    public class ItemViewSettings
    {
        public ItemViewSettings()
        {
            ShowBreadcrumb = true;
            NoImageImageFile = LayoutSettings.NoImageImageFile;

            ShowTitle = true;
            ShowItemNo = false;
            ShowBrandName = true;
            ShowPrice = true;
            ShowDefaultPrice = true;

            ShowBuyButton = true;
        }

        public bool ShowBreadcrumb { get; set; }

        public string NoImageImageFile { get; set; }


        public bool ShowTitle { get; set; }

        public bool ShowItemNo { get; set; }

        public bool ShowBrandName { get; set; }

        public bool ShowPrice { get; set; }

        public bool ShowDefaultPrice { get; set; }

        public bool ShowBuyButton { get; set; }
    }
}
