using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nCode.CMS.Models
{
    /// <summary>
    /// Represents a CMS Content Part.
    /// </summary>
    [Table("CMS_ContentPartInstance")]
    public class ContentPartInstance
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
        /// Gets the root Container ID, i.e, if ContainerID is a heirachy the topmost Container in that hierarchy. Otherwise this shoud just be equal to ContainerID.
        /// </summary>
        public Guid RootContainerID { get; set; }

        /// <summary>
        /// Gets or sets the Container ID, e.g. a Content Page ID, a Content Block ID, etc.
        /// </summary>
        [Index("IX_CMS_ContentPartInstance_ContainerID_DisplayIndex", 1, IsUnique = true)]
        public Guid ContainerID { get; set; }

        /// <summary>
        /// Gets or sets the Display Index, i.e. the order of the Contant Part within the container.
        /// </summary>
        [Index("IX_CMS_ContentPartInstance_ContainerID_DisplayIndex", 2, IsUnique = true)]
        public int DisplayIndex { get; set; }

        /// <summary>
        /// Gets or sets the Content Part ID.
        /// </summary>
        public Guid ContentPartID { get; set; }


        /// <summary>
        /// Gets or sets the Content Part.
        /// </summary>
        [ForeignKey("ContentPartID")]
        public virtual ContentPart ContentPart { get; set; }
    }
}
