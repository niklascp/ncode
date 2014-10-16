/* Internal Code - Disable Missing Comment Warnings. */
#pragma warning disable 1591

using nCode.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Ajax
{
    [Serializable]
    public class ItemInfo
    {
        public Guid ID { get; set; }   
        public string ItemNo { get; set; }
        public bool IsActive { get; set; }
        public Guid? Category { get; set; }
        public Guid? Brand { get; set; }     
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public VariantMode VariantMode { get; set; }
        public int? StockQty { get; set; }
        public int? ReservedQty { get; set; }
    }
}
