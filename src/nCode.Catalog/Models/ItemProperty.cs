using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Models
{
    /// <summary>
    /// Represents an Item Property.
    /// </summary>
    [Table("Catalog_ItemProperty")]
    public class CatalogItemProperty
	{
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
		[Key]
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the item unique identifier.
        /// </summary>
        public Guid ItemID { get; set; }

        /// <summary>
        /// Gets or sets the item (Navigation Property for ItemID).
        /// </summary>
        [ForeignKey("ItemID")]
        public CatalogItem Item { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Required]
        [MaxLength]
        [Column(TypeName = "ntext")]
        public string Value { get; set; }
	}
}
