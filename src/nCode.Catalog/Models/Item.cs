using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nCode.Catalog.Models
{
    /// <summary>
    /// Represents a Item.
    /// </summary>
    [Table("Catalog_Item")]
    [DataContract(Name = "Item")]
    public class CatalogItem 
    {
        [Key]
        [DataMember]
        public Guid ID { get ; set;}

        [Required]
        [DataMember]
        public DateTime Created { get; set; }

        [Required]
        [DataMember]
        public DateTime Modified { get; set; }

        //[UniqueKey]
        [DataMember]
        [Required]
        [MaxLength(255)]
        public string ItemNo { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        /* Category  */

        [Column("Category")]
        [DataMember]
        public Guid? CategoryID { get; set; }

        [Column("Index")]
        [DataMember]
        public int CategoryIndex { get; set; }

        /* Brand  */

        [Column("Brand")]
        [DataMember]
        public Guid? BrandID { get; set; }

        [DataMember]
        public int BrandIndex { get; set; }

        /* Variants */

        [DataMember]
        public VariantMode VariantMode { get; set; }

        /* Stock Control */

        [DataMember]
        public bool UseStockControl { get; set; }

        [DataMember]
        public int StockQuantity { get; set; }

        [DataMember]
        public int ReservedQuantity { get; set; }

        [DataMember]
        public bool IsSoldOut { get; set; }

        /* Order Quantities */

        [DataMember]
        public int? MinimumOrderQuantity { get; set; }

        [DataMember]
        public int? MultipleOrderQuantity { get; set; }

        /* VAT */

        [DataMember]
        [MaxLength(20)]
        public string VatGroupCode { get; set; }

        /* Cost Price */

        [DataMember]
        [MaxLength(20)]
        public string CostCurrencyCode { get; set; }

        public decimal CostPrice { get; set; }

        /* Weight and unit */

        [DataMember]
        public decimal Weight { get; set; }

        [DataMember]
        [MaxLength(20)]
        public string UnitCode { get; set; }

        /* Misc */

        [DataMember]
        public bool OnSale { get; set; }

        [DataMember]
        public virtual ICollection<CatalogItemLocalization> Localizations { get; set; }

        [DataMember]
        public virtual ICollection<CatalogItemProperty> Properties { get; set; }

        //public ICollection<ItemImage> Images { get; set; }

        //public ICollection<ItemProperty> Properties { get; set; }

        //public ICollection<ItemVariantType> ItemVariantTypes { get; set; }
        
        //public ICollection<ItemListPrice> ListPrices { get; set; }

        //[ForeignKey("BrandID")]
        //public Brand Brand { get; set; }

        //[ForeignKey("CategoryID")]
        //public Category Category { get; set; }


    }
	
}
