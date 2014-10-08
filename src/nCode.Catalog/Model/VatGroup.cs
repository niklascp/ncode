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
    [Table("Catalog_VatGroup")]
    public class VatGroup
    {
        public Guid ID { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Required]
        public string Code { get; set; }

        public bool IsDefault { get; set; }

        public decimal Rate { get; set; }
    }
}
