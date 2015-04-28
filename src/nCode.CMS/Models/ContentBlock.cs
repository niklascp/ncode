using nCode.Data;
using nCode.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace nCode.CMS.Models
{
    /// <summary>
    /// Represents a CMS Content Part.
    /// </summary>
    [Table("CMS_ContentBlock")]
    public class ContentBlock
    {

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [Key]
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the Date and Time for when this Content Block was Created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the Date and Time for when this Content Block was last Modified.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the Code for this Content Block.
        /// </summary>
        [Required, MaxLength(255)]
        public string Code { get; set; }
    }
}
