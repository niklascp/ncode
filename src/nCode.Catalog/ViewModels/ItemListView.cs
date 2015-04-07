using nCode.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.ViewModels
{
    public class ItemListView
    {
        public Guid ID { get; set; }
        public string ItemNo { get; set; }
        public string Title { get; set; }
        public ItemListPrice ListPrice { get; set; }
        public ItemListPrice DefaultListPrice { get; set; }
        public bool OnSale { get; set; }
        public bool IsAvailable { get; set; }
        public VariantMode VariantMode { get; set; }

        public Guid? CategoryID { get; set; }
        public string CategoryTitle { get; set; }
        public int CategoryIndex { get; set; }

        public Guid? BrandID { get; set; }
        public string BrandName { get; set; }
        public int BrandIndex { get; set; }

        public string ImageFile { get; set; }
    }
}
