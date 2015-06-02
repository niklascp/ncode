using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.ViewModels
{
    public class ItemListPriceView : IEquatable<ItemListPriceView>
    {     
        public string CurrencyCode { get; set; }

        public string PriceGroupCode { get; set; }

        public decimal Price { get; set; }

        public bool MultiplePrices { get; set; }

        public override int GetHashCode()
        {
            return CurrencyCode.GetHashCode() ^ PriceGroupCode.GetHashCode() ^ Price.GetHashCode() ^ MultiplePrices.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return base.Equals(other as ItemListPriceView);
        }

        public bool Equals(ItemListPriceView other)
        {
            return other != null && this.CurrencyCode == other.CurrencyCode && this.PriceGroupCode == other.PriceGroupCode && this.Price == other.Price && this.MultiplePrices == other.MultiplePrices;
        }
    }
}
