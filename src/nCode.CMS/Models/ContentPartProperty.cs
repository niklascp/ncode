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

namespace nCode.CMS.Models
{    
    /// <summary>
    /// Represents a CMS Content Part Property.
    /// </summary>
    [Table("CMS_ContentPartProperty")]
    public class ContentPartProperty
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
        /// Gets or sets the ID for the Content Part.
        /// </summary>
        public Guid ContentPartID { get; set; }

        /// <summary>
        /// Gets or sets the Content Part.
        /// </summary>
        [ForeignKey("ContentPartID")]
        public virtual ContentPart ContentPart { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [MaxLength(255)]
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
    }
}
