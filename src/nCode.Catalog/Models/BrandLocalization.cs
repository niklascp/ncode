using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.Models
{
    [Table("Catalog_BrandLocalization")]
    public class BrandLocalization
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [Key]
        public Guid ID { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [ForeignKey("BrandID")]
        public Brand Brand { get; set; }

        public Guid BrandID { get; set; }

        public string Culture { get; set; }

        public string Description { get; set; }

        public string SeoKeywords { get; set; }

        public string SeoDescription { get; set; }
    }
}
