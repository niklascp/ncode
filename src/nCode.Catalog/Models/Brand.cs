using nCode.Data;
using nCode.Metadata;
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
    public class Brand : IMetadataContext
    {
        private static Guid objectTypeId = new Guid("34054c40-98fb-40b3-8755-000aee62e5b0");

        /// <summary>
        /// Gets the object type unique identifier.
        /// </summary>
        /// <value>
        /// The object type unique identifier.
        /// </value>
        public static Guid ObjectTypeId { get { return objectTypeId; } }

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

        [Column("Index")]
        public int DisplayIndex { get; set; }

        [SetExistingRows("true")]
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

        [ForeignKey("ParentID"), CascadeRule(willCascadeOnDelete: false)]
        public Brand Parent { get; set; }


        /// <summary>
        /// Gets the Metadata Property given by the given key, or a default value if the Property is not defined.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty<T>(objectTypeId, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets the Metadata Property given by the given key to the given value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty<T>(objectTypeId, ID, key, value);
        }
    }
}
