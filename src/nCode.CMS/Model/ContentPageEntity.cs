using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nCode.UI;


namespace nCode.CMS.Model
{
    /// <summary>
    /// Represents a CMS Content Page.
    /// </summary>
    [Table("CMS_ContentPage")]
    public class ContentPageEntity
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
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        [Column("Language")]
        [MaxLength(10)]
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets the parent unique identifier.
        /// </summary>
        /// <value>
        /// The parent unique identifier.
        /// </value>
        [Column("ParentID")]
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the display index.
        /// </summary>
        /// <value>
        /// The display index.
        /// </value>
        [Column("DisplayIndex")]
        public int DisplayIndex { get; set; }

        /// <summary>
        /// Gets or sets the static title.
        /// </summary>
        /// <value>
        /// The static title.
        /// </value>
        [MaxLength(255)]
        public string StaticTitle { get; set; }

        /// <summary>
        /// Gets or sets the static path.
        /// </summary>
        /// <value>
        /// The static path.
        /// </value>
        [MaxLength(255)]
        public string StaticPath { get; set; }

        /// <summary>
        /// Gets or sets the content type unique identifier.
        /// </summary>
        /// <value>
        /// The content type unique identifier.
        /// </value>
        [Column("ContentType")]
        public Guid? ContentTypeID { get; set; }

        /// <summary>
        /// Gets or sets the content of the page.
        /// </summary>
        /// <value>
        /// The content of the page.
        /// </value>
        [Column("PageContent")]
        public string PageContent { get; set; }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>
        /// The link URL.
        /// </value>
        [MaxLength(255)]
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the link mode.
        /// </summary>
        /// <value>
        /// The link mode.
        /// </value>
        public LinkMode LinkMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is protected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is protected]; otherwise, <c>false</c>.
        /// </value>
        public bool IsProtected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show information menu].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show information menu]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowInMenu { get; set; }

        /// <summary>
        /// Gets or sets the valid from.
        /// </summary>
        /// <value>
        /// The valid from.
        /// </value>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the valid automatic.
        /// </summary>
        /// <value>
        /// The valid automatic.
        /// </value>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title mode.
        /// </summary>
        /// <value>
        /// The title mode.
        /// </value>
        public TitleMode TitleMode { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        [MaxLength(255)]
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the master page file.
        /// </summary>
        /// <value>
        /// The master page file.
        /// </value>
        [MaxLength(50)]
        public string MasterPageFile { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        //public virtual ICollection<ContentPart> ContentParts { get; set; }
    }
}
