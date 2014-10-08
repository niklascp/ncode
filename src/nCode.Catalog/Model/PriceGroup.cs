using nCode.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Model
{
    /// <summary>
    /// Represents a PriceGroup.
    /// </summary>
    [Table("Catalog_PriceGroup")]
    public class PriceGroup
    {
        /// <summary>
        /// Gets the Id of this Sale Channel.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [MaxLength(255)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether prices in this Price Group at statet inclusive Vat. 
        /// </summary>
        public bool PricesIncludeVat { get; set; }
    }
}
