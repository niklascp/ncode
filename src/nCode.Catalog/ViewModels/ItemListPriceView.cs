using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.ViewModels
{
    public class ItemListPriceView
    {     
        public string CurrencyCode { get; set; }

        public string PriceGroupCode { get; set; }

        public decimal Price { get; set; }

        public bool MultiplePrices { get; set; }
    }
}
