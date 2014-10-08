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
    /// Represents a Sales Channel.
    /// </summary>
    [Table("Catalog_SalesChannel")]
    [Rename("Catalog_SalesChannels")]
    [UniqueKey("Code")]
    [UniqueKey("Guid")]
    public class SalesChannel
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
        public Guid Guid { get ; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [MaxLength(20)]
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
        /// Gets or sets the price group code.
        /// </summary>
        /// <value>
        /// The price group code.
        /// </value>
        [MaxLength(20)]
        public string PriceGroupCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show prices including vat.
        /// </summary>
        /// <value>
        /// <c>true</c> if to show prices including vat; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPricesIncludingVat { get; set; }

        /// <summary>
        /// Gets or sets the name of the order confirm template.
        /// </summary>
        /// <value>
        /// The name of the order confirm template.
        /// </value>
        [MaxLength(255)]
        public string OrderConfirmTemplateName { get; set; }

        /// <summary>
        /// Gets or sets the name of the invoice template.
        /// </summary>
        /// <value>
        /// The name of the invoice template.
        /// </value>
        [MaxLength(255)]
        public string InvoiceTemplateName { get; set; }
    }
}
