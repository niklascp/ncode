using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nCode.Catalog.Models;
using nCode.Catalog.UI;
using Newtonsoft.Json;

namespace nCode.Catalog.ViewModels
{
    public class ItemListView
    {
        public Guid ID { get; set; }
        public string ItemNo { get; set; }
        public bool IsActive { get; set; }

        public string Title { get; set; }
        public ItemListPriceView ListPrice { get; set; }
        public ItemListPriceView DefaultListPrice { get; set; }
        public bool OnSale { get; set; }
        public VariantMode VariantMode { get; set; }

        public Guid? CategoryID { get; set; }
        public string CategoryTitle { get; set; }
        public int CategoryIndex { get; set; }

        public Guid? BrandID { get; set; }
        public string BrandName { get; set; }
        public int BrandIndex { get; set; }

        public string ImageFile { get; set; }

        /* Stock Management */
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? StockQuantity { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ReservedQuantity { get; set; }
        public bool IsAvailable { get; set; }

        public string Url
        {
            get { return SeoUtilities.GetItemUrl(ItemNo, Title); }
        }
    }
}
