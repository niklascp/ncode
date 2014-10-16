using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.Models
{
    [Table("Catalog_Brand")]
    public class Brand
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

        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [MaxLength(255), Required]
        public string Name { get; set; }

        public int DisplayIndex { get; set; }

        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the Image.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [MaxLength(255)]
        public string Image1 { get; set; }

        /// <summary>
        /// Gets or sets the Image.
        /// </summary>
        /// <value>
        /// The Image.
        /// </value>
        [MaxLength(255)]
        public string Image2 { get; set; }
        /// <summary>
        /// Gets or sets the Image.
        /// </summary>
        /// <value>
        /// The Image.
        /// </value>
        [MaxLength(255)]
        public string Image3 { get; set; }

        public Brand Parent { get; set; }

    }
}
