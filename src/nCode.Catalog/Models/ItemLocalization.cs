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
    /// Represents a Item Localization.
    /// </summary>
    [Table("Catalog_ItemLocalization")]
    [DataContract(Name = "ItemLocalization")]
    public class CatalogItemLocalization	
	{		
		[Key]
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public DateTime Modified { get; set; }

        [Column("Item")]
        [DataMember]
        public Guid ItemID { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string Culture { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string Title { get; set; }

        [DataMember]
        [MaxLength]
        public string Description { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string SeoKeywords { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string SeoDescription { get; set; }

        [ForeignKey("ItemID")]
        public CatalogItem Item { get; set; }	
	}
}
