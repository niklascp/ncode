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
    [Table("CMS_ContentBlockLocalization")]
    public class ContentBlockLocalization
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
        /// Gets or sets the unique identifier of the Content Block that this Localization belongs to.
        /// </summary>
        public Guid ContentBlockID { get; set; }

        /// <summary>
        /// Gets or sets the Culture for this Content Block Localization, or null for the Default Culture.
        /// </summary>
        [MaxLength(255)]
        public string Cultre { get; set; }

        /// <summary>
        /// Gets or sets the Content for this Content Block Localization.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the Content Block that this Localization belongs to.
        /// </summary>
        public virtual ContentBlock ContentBlock { get; set; }
    }
}
